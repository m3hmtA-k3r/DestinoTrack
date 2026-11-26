using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Options;
using DestinoTrack.Business.Services.Pricing;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.DataAccess.Repositories.CargoMovements;
using DestinoTrack.DataAccess.Repositories.Cargos;
using DestinoTrack.DataAccess.Repositories.Customers;
using DestinoTrack.DataAccess.Repositories.Deliveries;
using DestinoTrack.DataAccess.Repositories.DeliveryExceptions;
using DestinoTrack.DataAccess.Repositories.Employees;
using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Frozen;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace DestinoTrack.Business.Services.Cargos
{
    public class CargoService(ICargoRepository _cargoRepository,
                          ICargoMovementRepository _cargoMovementRepository,
                          ICustomerRepository _customerRepository,
                          IEmployeeRepository _employeeRepository,
                          IDeliveryRepository _deliveryRepository,
                          IDeliveryExceptionRepository _deliveryExceptionRepository,
                          ICargoPricingService _cargoPricingService,
                          ITrackCodeGenerator _trackCodeGenerator,
                          ICurrentUserAccessor _currentUser,
                          IBarcodeService _barcodeService,
                          IPublicTrackingUrl _publicTrackingUrl,
                          IOptions<CargoSettings> _cargoSettings,
                          IStringLocalizer<SharedResource> _localizer) : ICargoService
    {
        // izin verilen geçisler Kural kodda durur, veritabanından deghiştirilemez.
        // Listede olmayan her geçiş yasaktır — Teslim Edildi ve İade Edildi son durumlardır.
        private static readonly FrozenDictionary<CargoStatus, CargoStatus[]> AllowedTransitions =
            new Dictionary<CargoStatus, CargoStatus[]>
            {
                [CargoStatus.Created] = [CargoStatus.AtOriginBranch],
                [CargoStatus.AtOriginBranch] = [CargoStatus.InTransferCenter, CargoStatus.AtDestinationBranch],
                [CargoStatus.InTransferCenter] = [CargoStatus.InTransferCenter, CargoStatus.AtDestinationBranch],
                [CargoStatus.AtDestinationBranch] = [CargoStatus.OutForDelivery],
                [CargoStatus.OutForDelivery] = [CargoStatus.Delivered, CargoStatus.DeliveryFailed],
                [CargoStatus.DeliveryFailed] = [CargoStatus.OutForDelivery, CargoStatus.ReturnInProgress],
                [CargoStatus.ReturnInProgress] = [CargoStatus.ReturnedToSender],
                [CargoStatus.Delivered] = [],
                [CargoStatus.ReturnedToSender] = []
            }.ToFrozenDictionary();


        private static readonly CargoStatus[] ActiveStatuses = AllowedTransitions.Where(g => g.Value.Length > 0).Select(g => g.Key).ToArray();
        private static readonly CargoStatus[] FinalStatuses = AllowedTransitions.Where(g => g.Value.Length == 0).Select(g => g.Key).ToArray();
        private static readonly FrozenSet<CargoStatus> GatedStatuses =
            new[] { CargoStatus.OutForDelivery, CargoStatus.Delivered, CargoStatus.DeliveryFailed }.ToFrozenSet();



        public async Task<Guid> CreateAsync(CreateCargoDto createCargoDto, Guid performedByUserId)
        {
            // kurumsal gönderide müşteri onaylı ve aktif olmalı — askıdaki hesap adına kargo açılmaz
            if (createCargoDto.CustomerId.HasValue)
            {
                var customer = await _customerRepository.GetByIdAsync(createCargoDto.CustomerId.Value)
                               ?? throw new ValidationException(_localizer["CustomerNotFound"].Value);

                if (customer.Status != AccountStatus.Active)
                {
                    throw new ValidationException(_localizer["CustomerNotActive"].Value);
                }
            }

            //ücret ve tahmini teslim tarihi o anki tarifeyle hesaplanır.
            // Şubeler burada da doğrulanır: olmayan şube ya da tarifesiz güzergah buradan geri döner
            var quote = await _cargoPricingService.GetQuoteAsync(new CargoQuoteRequestDto
            {
                OriginBranchId = createCargoDto.OriginBranchId,
                DestinationBranchId = createCargoDto.DestinationBranchId,
                CargoType = createCargoDto.CargoType,
                Weight = createCargoDto.Weight,
                Width = createCargoDto.Width,
                Height = createCargoDto.Height,
                Length = createCargoDto.Length
            });

            var trackCode = await _trackCodeGenerator.GenerateAsync();

            var cargo = new Cargo
            {
                TrackCode = trackCode,
                Barcode = trackCode.Replace("-", string.Empty),   //barkod aynı numaranın tirelerden arınmış hali
                CargoStatus = CargoStatus.Created,
                ShipmentDate = DateTime.UtcNow,

                CargoType = createCargoDto.CargoType,
                PaymentType = createCargoDto.PaymentType,
                Weight = createCargoDto.Weight,
                Width = createCargoDto.Width,
                Height = createCargoDto.Height,
                Length = createCargoDto.Length,

                // Fiyat kargoya yazılır: tarife sonradan değişse bile bu kargonun ücreti değişmez
                Desi = quote.Desi,
                Price = quote.Price,
                CurrencyCode = quote.CurrencyCode,
                EstimatedArrivalDate = quote.EstimatedArrivalDate,
                IsPaid = createCargoDto.PaymentType == PaymentType.SenderPays,

                OriginBranchId = createCargoDto.OriginBranchId,
                DestinationBranchId = createCargoDto.DestinationBranchId,

                //  bilgi kargoda, hesap bağı isteğe bağlı
                SenderName = createCargoDto.SenderName.Trim(),
                SenderPhone = createCargoDto.SenderPhone.Trim(),
                SenderAddress = createCargoDto.SenderAddress.Trim(),
                SenderId = createCargoDto.SenderId,

                ReceiverName = createCargoDto.ReceiverName.Trim(),
                ReceiverPhone = createCargoDto.ReceiverPhone.Trim(),
                ReceiverAddress = createCargoDto.ReceiverAddress.Trim(),
                ReceiverId = createCargoDto.ReceiverId,

                CustomerId = createCargoDto.CustomerId
            };

            await _cargoRepository.CreateAsync(cargo);

            // ilk hareket — önceki durum yok, kargo çıkış şubesinde doğuyor
            await _cargoMovementRepository.CreateAsync(new CargoMovement
            {
                CargoId = cargo.Id,
                OldStatus = null,
                CargoStatus = CargoStatus.Created,
                MovementDate = DateTime.UtcNow,
                BranchId = cargo.OriginBranchId,
                PerformedByUserId = performedByUserId,
                Description = _localizer["CargoCreatedMovement"].Value
            });

            return cargo.Id;
        }

        public async Task<PagedResult<ResultCargoDto>> GetPagedAsync(CargoStatus? status = null, string? search = null, int page = 1,
                                                             CargoDirection? direction = null)
        {
            page = page < 1 ? 1 : page;
            var scope = await GetScopeAsync();

            var (cargos, totalCount) = await _cargoRepository.GetPagedScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted,
                status, search, page, Paging.PageSize, direction);

            // Son sayfadaki tek kayıt silinince o sayfa boş kalır → varsa yeni son sayfa gösterilir
            if (cargos.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (cargos, totalCount) = await _cargoRepository.GetPagedScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted,
                    status, search, page, Paging.PageSize, direction);
            }


            return new PagedResult<ResultCargoDto>
            {
                Items = cargos.Select(c => new ResultCargoDto
                {
                    Id = c.Id,
                    TrackCode = c.TrackCode,
                    CargoStatus = c.CargoStatus,
                    CargoType = c.CargoType,
                    SenderName = c.SenderName,
                    ReceiverName = c.ReceiverName,
                    OriginBranchName = c.OriginBranch?.Name,
                    DestinationBranchName = c.DestinationBranch?.Name,
                    Price = c.Price,
                    CurrencyCode = c.CurrencyCode,
                    IsPaid = c.IsPaid,
                    ShipmentDate = c.ShipmentDate,
                    EstimatedArrivalDate = c.EstimatedArrivalDate
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CargoDetailDto> GetDetailAsync(Guid id)
        {
            var scope = await GetScopeAsync();

            var cargo = await _cargoRepository.GetDetailScopedAsync(id, scope.BranchId, scope.CountryId,
                                                                    scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted);
            if (cargo == null)
            {
                // kayıt var ama kapsam dışıysa yetki hatası, hiç yoksa bulunamadı
                var exists = await _cargoRepository.GetByIdAsync(id) != null;
                throw new ValidationException(_localizer[exists ? "UnauthorizedRecord" : "CargoNotFound"].Value);
            }

            return new CargoDetailDto
            {
                Id = cargo.Id,
                TrackCode = cargo.TrackCode,
                Barcode = cargo.Barcode,
                CargoStatus = cargo.CargoStatus,
                CargoType = cargo.CargoType,

                SenderName = cargo.SenderName,
                SenderPhone = cargo.SenderPhone,
                SenderAddress = cargo.SenderAddress,
                ReceiverName = cargo.ReceiverName,
                ReceiverPhone = cargo.ReceiverPhone,
                ReceiverAddress = cargo.ReceiverAddress,
                CustomerTitle = cargo.Customer?.Title,

                OriginBranchName = cargo.OriginBranch?.Name,
                DestinationBranchName = cargo.DestinationBranch?.Name,
                Weight = cargo.Weight,
                Desi = cargo.Desi,

                Price = cargo.Price,
                CurrencyCode = cargo.CurrencyCode,
                IsPaid = cargo.IsPaid,
                PaymentType = cargo.PaymentType,
                ShipmentDate = cargo.ShipmentDate,
                EstimatedArrivalDate = cargo.EstimatedArrivalDate,

                CourierName = cargo.Courier == null ? null : $"{cargo.Courier.FirstName} {cargo.Courier.LastName}",
                DeliveryCode = _currentUser.Role == RoleNames.Admin ? cargo.DeliveryCode : null,
                RecipientName = cargo.Delivery?.RecipientName,
                DeliveredAt = cargo.Delivery?.DeliveredAt,
                FailedAttemptCount = cargo.DeliveryExceptions?.Count ?? 0,

                // çizelge eskiden yeniye — ilk satır "Oluşturuldu", son satır güncel durum
                Movements = (cargo.Movements ?? new List<CargoMovement>())
                    .OrderBy(m => m.MovementDate)
                    .ThenBy(m => m.CargoStatus)
                    .Select(m => new CargoMovementDto
                    {
                        MovementDate = m.MovementDate,
                        OldStatus = m.OldStatus,
                        CargoStatus = m.CargoStatus,
                        Description = m.Description,
                        DelayReason = m.DelayReason,
                        BranchName = m.Branch?.Name,
                        PerformedByName = m.PerformedByUser == null
                            ? null
                            : $"{m.PerformedByUser.FirstName} {m.PerformedByUser.LastName}"
                    }).ToList()
            };
        }

        public async Task<CargoLabelDto> GetLabelAsync(Guid id)
        {
            var scope = await GetScopeAsync();

            var cargo = await _cargoRepository.GetLabelScopedAsync(id, scope.BranchId, scope.CountryId,
                                                                   scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted);
            if (cargo == null)
            {
                // detayda olduğu gibi: kayıt var ama kapsam dışıysa yetki hatası, hiç yoksa bulunamadı
                var exists = await _cargoRepository.GetByIdAsync(id) != null;
                throw new ValidationException(_localizer[exists ? "UnauthorizedRecord" : "CargoNotFound"].Value);
            }

            // QR takip sayfasının adresini taşır. Adres üretilemiyorsa (istek yoksa)
            // eskisi gibi takip numarasının kendisi yazılır — etiket her koşulda basılabilir
            var qrContent = _publicTrackingUrl.For(cargo.TrackCode) ?? cargo.TrackCode;


            return new CargoLabelDto
            {
                Id = cargo.Id,
                TrackCode = cargo.TrackCode,
                Barcode = cargo.Barcode,
                BarcodeSvg = _barcodeService.Code128Svg(cargo.Barcode),
                QrSvg = _barcodeService.QrSvg(qrContent),
                QrContent = qrContent,
                CargoType = cargo.CargoType,

                SenderName = cargo.SenderName,
                SenderPhone = cargo.SenderPhone,
                SenderAddress = cargo.SenderAddress,
                CustomerTitle = cargo.Customer?.Title,
                ReceiverName = cargo.ReceiverName,
                ReceiverPhone = cargo.ReceiverPhone,
                ReceiverAddress = cargo.ReceiverAddress,

                OriginBranchName = cargo.OriginBranch?.Name,
                OriginBranchCode = cargo.OriginBranch?.Code,
                OriginCityName = cargo.OriginBranch?.City?.Name,
                DestinationBranchName = cargo.DestinationBranch?.Name,
                DestinationBranchCode = cargo.DestinationBranch?.Code,
                DestinationCityName = cargo.DestinationBranch?.City?.Name,
                DestinationCountryName = cargo.DestinationBranch?.City?.Country?.Name,

                Weight = cargo.Weight,
                Width = cargo.Width,
                Height = cargo.Height,
                Length = cargo.Length,
                Desi = cargo.Desi,

                Price = cargo.Price,
                CurrencyCode = cargo.CurrencyCode,
                PaymentType = cargo.PaymentType,
                IsPaid = cargo.IsPaid,

                ShipmentDate = cargo.ShipmentDate,
                EstimatedArrivalDate = cargo.EstimatedArrivalDate,
                PrintedAt = DateTime.UtcNow
            };
        }

        public async Task<PublicTrackingDto?> GetPublicTrackingAsync(string trackCode)
        {
            // Biçim denetimi önce: hatalı metin için sorgu açılmaz
            trackCode = (trackCode ?? string.Empty).Trim().ToUpperInvariant();
            if (!TrackCodePattern.IsMatch(trackCode))
            {
                return null;
            }

            var cargo = await _cargoRepository.GetPublicByTrackCodeAsync(trackCode);
            if (cargo == null)
            {
                return null;
            }

            var movements = (cargo.Movements ?? new List<CargoMovement>())
                .OrderBy(m => m.MovementDate)
                .ThenBy(m => m.CargoStatus)
                .ToList();

            // tutar yalnız kapıda ödenecekse görünür
            var kapidaOdenecek = cargo.PaymentType == PaymentType.ReceiverPays && !cargo.IsPaid;

            return new PublicTrackingDto
            {
                TrackCode = cargo.TrackCode,
                CargoStatus = cargo.CargoStatus,
                CargoType = cargo.CargoType,

                ShipmentDate = cargo.ShipmentDate,
                EstimatedArrivalDate = cargo.EstimatedArrivalDate,

                // Teslim zamanı hareketten: Delivery satırında teslim alan kişinin adı var, oraya hiç dokunmuyoruz
                DeliveredAt = movements.LastOrDefault(m => m.CargoStatus == CargoStatus.Delivered)?.MovementDate,

                OriginCityName = cargo.OriginBranch?.City?.Name,
                DestinationCityName = cargo.DestinationBranch?.City?.Name,

                AmountDueOnDelivery = kapidaOdenecek ? cargo.Price : null,
                CurrencyCode = kapidaOdenecek ? cargo.CurrencyCode : null,

                Movements = movements.Select(m => new PublicMovementDto
                {
                    MovementDate = m.MovementDate,
                    CargoStatus = m.CargoStatus,
                    BranchName = m.Branch?.Name,
                    CityName = m.Branch?.City?.Name,
                    DelayReason = m.DelayReason
                }).ToList()
            };
        }

        // {Önek}-{Yıl}-{6 rakam}. Uymayan metin veritabanına hiç gitmez
        private static readonly Regex TrackCodePattern = new(@"^[A-Z]{2,5}-\d{4}-\d{6}$", RegexOptions.Compiled);

        public bool CanTransition(CargoStatus from, CargoStatus to)
        {
            return AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
        }


        public async Task<CargoMovementTargetDto> FindForMovementAsync(string trackCode)
        {
            // ToUpperInvariant: tr-TR'de ToUpper() 'i' harfini 'İ' yapar, takip numarası bozulur
            var code = (trackCode ?? string.Empty).Trim().ToUpperInvariant();

            var scope = await GetScopeAsync();

            var cargo = code.Length == 0 || !TrackCodePattern.IsMatch(code)
                ? null
                : await _cargoRepository.GetByTrackCodeScopedAsync(code, scope.BranchId, scope.CountryId,
                                                                   scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted);
            if (cargo == null)
            {
                // Kapsam dışındaki kargoda da "bulunamadı" diyoruz: "var ama yetkiniz yok" demek,
                // başka şubenin takip numarasının geçerli olduğunu doğrulamak olurdu
                throw new ValidationException(_localizer["TrackNotFound"].Value);
            }


            // Haritadaki bütün hedefler, sonra ekrandan yazılabilenler — kendi akışı olanlar süzülüyor (D71)
            var hedefler = AllowedTransitions.TryGetValue(cargo.CargoStatus, out var targets) ? targets : [];
            var serbest = hedefler.Where(s => !GatedStatuses.Contains(s)).ToList();


            return new CargoMovementTargetDto
            {
                Id = cargo.Id,
                TrackCode = cargo.TrackCode,
                CurrentStatus = cargo.CargoStatus,
                AllowedStatuses = serbest,
                HandledInDeliveryScreen = serbest.Count == 0 && hedefler.Length > 0,
                CourierId = cargo.CourierId,
                OriginBranchName = cargo.OriginBranch.Name,
                DestinationBranchName = cargo.DestinationBranch.Name,
                ReceiverName = cargo.ReceiverName,
                LastMovementDate = cargo.Movements.Count == 0 ? null : cargo.Movements.Max(m => m.MovementDate)
            };
        }

        // Genel kapı: ekranlar buradan geçer. Kapılı durumlar buradan yazılamaz
        public async Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid branchId, Guid performedByUserId,
                                            string? description = null, DelayReason? delayReason = null)
        {
            if (GatedStatuses.Contains(newStatus))
            {
                throw new ValidationException(_localizer["StatusRequiresOwnFlow",
                    _localizer["CargoStatus_" + newStatus].Value].Value);
            }

            await WriteTransitionAsync(cargoId, newStatus, branchId, performedByUserId, description, delayReason);
        }

        private async Task WriteTransitionAsync(Guid cargoId, CargoStatus newStatus, Guid branchId, Guid performedByUserId,
                                                string? description = null, DelayReason? delayReason = null)
        {
            var cargo = await FindInScopeAsync(cargoId);


            if (!CanTransition(cargo.CargoStatus, newStatus))
            {
                // Mesajda iki durumun adı da geçsin: "Dağıtıma Çıktı durumundan Transfer Merkezinde durumuna geçilemez."
                throw new ValidationException(_localizer["InvalidStatusTransition",
                    _localizer["CargoStatus_" + cargo.CargoStatus].Value,
                    _localizer["CargoStatus_" + newStatus].Value].Value);
            }

            var oldStatus = cargo.CargoStatus;
            cargo.CargoStatus = newStatus;
            await _cargoRepository.UpdateAsync(cargo);

            //eski durum · yeni durum · şube · işlemi yapan — geçiş ile kayıt birlikte, biri olmadan diğeri olmaz
            await _cargoMovementRepository.CreateAsync(new CargoMovement
            {
                CargoId = cargo.Id,
                OldStatus = oldStatus,
                CargoStatus = newStatus,
                MovementDate = DateTime.UtcNow,
                BranchId = branchId,
                PerformedByUserId = performedByUserId,
                Description = description ?? string.Empty,
                DelayReason = delayReason
            });
        }

        public async Task<string> DispatchAsync(Guid cargoId, Guid courierEmployeeId, Guid branchId, Guid performedByUserId)
        {
            var cargo = await FindInScopeAsync(cargoId);


            // Kurye gerçekten var mı, aktif mi, kurye görevinde mi
            var courier = await _employeeRepository.GetByIdAsync(courierEmployeeId)
                          ?? throw new ValidationException(_localizer["EmployeeNotFound"].Value);

            if (!courier.IsActive || courier.JobType != EmployeeJobType.Courier)
            {
                throw new ValidationException(_localizer["CourierInvalid"].Value);
            }

            //kargo bu kuryeye atanır; teslim ederken aynı kurye aranacak
            cargo.CourierId = courier.Id;

            // kod bir kez üretilir — ikinci kez dağıtıma çıkarken aynı kod geçerli kalır
            cargo.DeliveryCode ??= RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();

            await _cargoRepository.UpdateAsync(cargo);

            // Durum geçişi ve hareket kaydı tek kapıdan: Varış Şubesinde → Dağıtıma Çıktı
            await WriteTransitionAsync(cargoId, CargoStatus.OutForDelivery, branchId, performedByUserId,
                                    _localizer["CargoDispatchedMovement", $"{courier.FirstName} {courier.LastName}"].Value);

            return cargo.DeliveryCode;
        }

        public async Task DeliverAsync(Guid cargoId, string deliveryCode, string recipientName, Guid courierEmployeeId,
                               Guid branchId, Guid performedByUserId)
        {
            var cargo = await FindInScopeAsync(cargoId);

            if (cargo.CourierId != courierEmployeeId)
            {
                throw new ValidationException(_localizer["CargoNotAssignedToCourier"].Value);
            }

            if (string.IsNullOrWhiteSpace(cargo.DeliveryCode) || cargo.DeliveryCode != deliveryCode?.Trim())
            {
                throw new ValidationException(_localizer["DeliveryCodeInvalid"].Value);
            }

            if (string.IsNullOrWhiteSpace(recipientName))
            {
                throw new ValidationException(_localizer["RecipientNameRequired"].Value);
            }

            await WriteTransitionAsync(cargoId, CargoStatus.Delivered, branchId, performedByUserId,
                                    _localizer["CargoDeliveredMovement", recipientName.Trim()].Value);

            await _deliveryRepository.CreateAsync(new Delivery
            {
                CargoId = cargo.Id,
                Code = cargo.DeliveryCode,
                RecipientName = recipientName.Trim(),
                DeliveredByEmployeeId = courierEmployeeId,
                DeliveredAt = DateTime.UtcNow
            });
            if (cargo.PaymentType == PaymentType.ReceiverPays && !cargo.IsPaid)
            {
                cargo.IsPaid = true;
                await _cargoRepository.UpdateAsync(cargo);
            }
        }

        public async Task FailDeliveryAsync(Guid cargoId, DeliveryFailureReason reason, string? description,
                                    Guid courierEmployeeId, Guid branchId, Guid performedByUserId)
        {
            var cargo = await FindInScopeAsync(cargoId);


            //denemeyi de kargonun kuryesi bildirir
            if (cargo.CourierId != courierEmployeeId)
            {
                throw new ValidationException(_localizer["CargoNotAssignedToCourier"].Value);
            }

            // Gerekçe "Diğer" ise ne olduğu yazılmalı, yoksa kayıt bir işe yaramaz
            if (reason == DeliveryFailureReason.Other && string.IsNullOrWhiteSpace(description))
            {
                throw new ValidationException(_localizer["FailureDescriptionRequired"].Value);
            }

            // Önce durum: Dağıtıma Çıktı değilse buraya gelinemez 
            await WriteTransitionAsync(cargoId, CargoStatus.DeliveryFailed, branchId, performedByUserId,
                                    _localizer["DeliveryFailedMovement", _localizer["DeliveryFailureReason_" + reason].Value].Value);

            //kaçıncı deneme olduğu satır sayısından gelir — ayrı sayaç tutulmaz, ikisi ayrışmasın
            var attemptNo = await _deliveryExceptionRepository.CountByCargoAsync(cargoId) + 1;

            await _deliveryExceptionRepository.CreateAsync(new DeliveryException
            {
                CargoId = cargo.Id,
                AttemptNo = attemptNo,
                Reason = reason,
                Description = description?.Trim(),
                AttemptedByEmployeeId = courierEmployeeId,
                AttemptedAt = DateTime.UtcNow
            });

            //sınıra ulaşıldıysa iade süreci kendiliğinden başlar
            if (attemptNo >= _cargoSettings.Value.MaxDeliveryAttempts)
            {
                await WriteTransitionAsync(cargoId, CargoStatus.ReturnInProgress, branchId, performedByUserId,
                                        _localizer["ReturnStartedMovement", attemptNo].Value);
            }

        }

        private async Task<CargoScope> GetScopeAsync()
        {
            if (_currentUser.Role == RoleNames.Courier)
            {
                // Kurye yalnız kendisine atanmış kargoları görür (D73) — şubesinin tamamını değil.
                // Personel kaydı yoksa kapsamsız kullanıcı gibi davranır: hiçbir şey görmez
                var employee = _currentUser.UserId is null
                    ? null
                    : await _employeeRepository.GetByAppUserAsync(_currentUser.UserId.Value);

                return new CargoScope { CourierEmployeeId = employee?.Id };
            }

            return _currentUser.Role switch
            {
                RoleNames.Admin => new CargoScope { IsUnrestricted = true },
                RoleNames.Manager => new CargoScope { CountryId = _currentUser.CountryId },
                RoleNames.Personel => new CargoScope { BranchId = _currentUser.BranchId },

                // Kurumsal çalışanı şirketinin kargolarını, bireysel müşteri kendi gönderdiklerini görür
                RoleNames.Customer => new CargoScope
                {
                    CustomerId = _currentUser.CustomerId,
                    UserId = _currentUser.CustomerId is null ? _currentUser.UserId : null
                },

                _ => new CargoScope()
            };
        }

        public Task<PagedResult<DeliveryRowDto>> GetReadyForDispatchAsync(int page = 1)
        {
            return DeliveryListAsync([CargoStatus.AtDestinationBranch, CargoStatus.DeliveryFailed],
                                     page, CargoDirection.Incoming);
        }

        public Task<PagedResult<DeliveryRowDto>> GetOnDeliveryAsync(int page = 1)
        {
            return DeliveryListAsync([CargoStatus.OutForDelivery], page);
        }

        // İki listenin ortak gövdesi: yalnız durum ve yön değişiyor
        private async Task<PagedResult<DeliveryRowDto>> DeliveryListAsync(CargoStatus[] statuses, int page,
                                                                          CargoDirection? direction = null)
        {
            page = page < 1 ? 1 : page;
            var scope = await GetScopeAsync();

            var (cargos, totalCount) = await _cargoRepository.GetDeliveryListScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted,
                statuses, page, Paging.PageSize, direction);

            return new PagedResult<DeliveryRowDto>
            {
                Items = cargos.Select(c => new DeliveryRowDto
                {
                    Id = c.Id,
                    TrackCode = c.TrackCode,
                    CargoStatus = c.CargoStatus,

                    ReceiverName = c.ReceiverName,
                    ReceiverPhone = c.ReceiverPhone,
                    ReceiverAddress = c.ReceiverAddress,

                    DestinationBranchName = c.DestinationBranch?.Name,
                    EstimatedArrivalDate = c.EstimatedArrivalDate,

                    PaymentType = c.PaymentType,
                    Price = c.Price,
                    CurrencyCode = c.CurrencyCode,
                    IsPaid = c.IsPaid,

                    // Kurye kimliği satırdan geliyor: teslim/iade formdan gelen bir kimlikle yazılmıyor
                    CourierId = c.CourierId,
                    CourierName = c.Courier == null ? null : $"{c.Courier.FirstName} {c.Courier.LastName}",

                    AttemptCount = c.DeliveryExceptions?.Count ?? 0
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<CargoIssueRowDto>> GetIssuesAsync(CargoIssue? issue = null, int page = 1)
        {
            page = page < 1 ? 1 : page;
            var scope = await GetScopeAsync();

            var (cargos, totalCount) = await _cargoRepository.GetIssuesScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted,
                issue, page, Paging.PageSize);

            return new PagedResult<CargoIssueRowDto>
            {
                Items = cargos.Select(c => new CargoIssueRowDto
                {
                    Id = c.Id,
                    TrackCode = c.TrackCode,
                    CargoStatus = c.CargoStatus,

                    ReceiverName = c.ReceiverName,
                    DestinationBranchName = c.DestinationBranch?.Name,
                    EstimatedArrivalDate = c.EstimatedArrivalDate,

                    AttemptCount = c.DeliveryExceptions?.Count ?? 0,

                    // Son deneme neden başarısız oldu: satır "neden sorunlu" sorusunu kendi yanıtlasın
                    LastFailureReason = c.DeliveryExceptions == null || c.DeliveryExceptions.Count == 0
                        ? null
                        : c.DeliveryExceptions.OrderByDescending(d => d.AttemptNo).First().Reason,

                    CourierName = c.Courier == null ? null : $"{c.Courier.FirstName} {c.Courier.LastName}"
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<DailySummaryDto?> GetDailySummaryAsync()
        {
            var scope = await GetScopeAsync();

            // Kapsam sırası: şube daha dar, varsa o kazanır. Şubesi olmayan ama ülkesi
            // olan kullanıcı (Manager) ülke toplamını görür. İkisi de yoksa üretilecek sayı yok
            Dictionary<CargoStatus, int> bugun;

            if (_currentUser.BranchId is not null)
            {
                bugun = await _cargoMovementRepository.CountTodayByNewStatusAsync(_currentUser.BranchId.Value);
            }
            else if (scope.CountryId is not null)
            {
                bugun = await _cargoMovementRepository.CountTodayByCountryAsync(scope.CountryId.Value);
            }
            else
            {
                return null;
            }

            return new DailySummaryDto
            {
                ArrivedToday = Adet(bugun, CargoStatus.AtDestinationBranch),
                DispatchedToday = Adet(bugun, CargoStatus.AtOriginBranch),
                DeliveredToday = Adet(bugun, CargoStatus.Delivered),

                OnDelivery = await _cargoRepository.CountByStatusScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, CargoStatus.OutForDelivery),

                Delayed = await _cargoRepository.CountIssuesScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, CargoIssue.Delayed),

                Issues = await _cargoRepository.CountIssuesScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, null)
            };
        }

        public async Task<CustomerSummaryDto?> GetCustomerSummaryAsync()
        {
            var scope = await GetScopeAsync();

            // Pano "benim kargolarım" diyor: bu soruyu yalnız müşteri kapsamı yanıtlayabilir.
            // Şube ya da ülke kapsamıyla gelen birine o toplamı basmak yanlış sayı olurdu
            if (scope.CustomerId is null && scope.UserId is null)
            {
                return null;
            }

            var latest = await _cargoRepository.GetLatestScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted);

            return new CustomerSummaryDto
            {
                Active = await _cargoRepository.CountByStatusScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, ActiveStatuses),

                Delivered = await _cargoRepository.CountByStatusScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, CargoStatus.Delivered),

                LatestTrackCode = latest?.TrackCode,
                LatestId = latest?.Id,
                LatestStatus = latest?.CargoStatus,

                // Hareketler sorguda Include edildi: burada ek sorgu açılmıyor
                LatestMovementDate = latest?.Movements.Max(m => (DateTime?)m.MovementDate)
            };
        }

        public async Task<CustomerTimelineDto?> GetCustomerTimelineAsync()
        {
            var scope = await GetScopeAsync();

            if (scope.CustomerId is null && scope.UserId is null)
            {
                return null;
            }

            var cargo = await _cargoRepository.GetLatestScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted);

            // Hiç kargosu yok: bileşen sessiz kalacak, "henüz kargonuz yok"u VC11 yazıyor
            if (cargo is null)
            {
                return null;
            }

            var movements = (cargo.Movements ?? new List<CargoMovement>())
                .OrderBy(m => m.MovementDate)
                .ThenBy(m => m.CargoStatus)
                .ToList();

            return new CustomerTimelineDto
            {
                TrackCode = cargo.TrackCode,
                CargoStatus = cargo.CargoStatus,
                EstimatedArrivalDate = cargo.EstimatedArrivalDate,

                DeliveredAt = movements.LastOrDefault(m => m.CargoStatus == CargoStatus.Delivered)?.MovementDate,

                Movements = movements.Select(m => new CustomerMovementDto
                {
                    MovementDate = m.MovementDate,
                    CargoStatus = m.CargoStatus,
                    BranchName = m.Branch?.Name,
                    CityName = m.Branch?.City?.Name,
                    DelayReason = m.DelayReason
                }).ToList()
            };
        }

        public async Task<AdminSummaryDto?> GetAdminSummaryAsync()
        {
            var scope = await GetScopeAsync();

            // Pano bütün sistemin sayısını söylüyor: bu soruyu yalnız kısıtsız kapsam yanıtlayabilir.
            // Daralmış kapsama yanlış toplam basmaktansa hiç basma
            if (!scope.IsUnrestricted)
            {
                return null;
            }

            var teslim = await _cargoRepository.CountByStatusScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted, CargoStatus.Delivered);

            var sonuclanan = await _cargoRepository.CountByStatusScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted, FinalStatuses);

            return new AdminSummaryDto
            {
                TotalCargo = await _cargoRepository.CountScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted),

                ActiveCargo = await _cargoRepository.CountByStatusScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, ActiveStatuses),

                // Gecikme kuralı sorunlu kargolar listesiyle AYNI metottan: ekranda 5 diyorsa kart da 5 der
                DelayedCargo = await _cargoRepository.CountIssuesScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                    scope.IsUnrestricted, CargoIssue.Delayed),

                DeliveredCargo = teslim,
                FinalizedCargo = sonuclanan,

                // Hiç sonuçlanmamışsa oran YOK. 0 yazmak "hepsi başarısız" demek olurdu
                DeliveryRate = sonuclanan == 0 ? null : (double)teslim / sonuclanan
            };
        }


        public async Task<StatusDistributionDto?> GetStatusDistributionAsync()
        {
            var scope = await GetScopeAsync();

            if (!scope.IsUnrestricted)
            {
                return null;
            }

            var sayim = await _cargoRepository.CountGroupedByStatusScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted);

            var toplam = sayim.Values.Sum();

            return new StatusDistributionDto
            {
                Total = toplam,

                Rows = AllowedTransitions.Keys.OrderBy(durum => durum).Select(durum => new StatusCountDto
                {
                    Status = durum,
                    Count = Adet(sayim, durum),
                    Share = toplam == 0 ? 0 : (double)Adet(sayim, durum) / toplam
                }).ToList()
            };
        }


        public async Task<WeeklyTrafficDto?> GetWeeklyTrafficAsync()
        {
            var scope = await GetScopeAsync();

            if (!scope.IsUnrestricted)
            {
                return null;
            }

            // "Son 7 gün" = bugün DAHİL, geriye 6 gün. Sınır UTC: günlük özet de UTC okuyor,
            // iki yerde iki ayrı "bugün" çıkmasın
            var bugun = DateTime.UtcNow.Date;
            var baslangic = bugun.AddDays(-6);

            var sayim = await _cargoRepository.CountByShipmentDayScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted, baslangic, bugun.AddDays(1));

            // Yedi günün yedisi de satır alıyor: kargosuz gün atlanırsa grafik boşluğu GİZLER
            var satirlar = Enumerable.Range(0, 7)
                .Select(i => baslangic.AddDays(i))
                .Select(gun => new DailyTrafficDto
                {
                    Date = gun,
                    Count = sayim.TryGetValue(gun, out var adet) ? adet : 0
                })
                .ToList();

            return new WeeklyTrafficDto
            {
                Rows = satirlar,

                // Çubuğun ölçeği EN YOĞUN gün: günler arası fark görünsün
                Max = satirlar.Max(x => x.Count)
            };
        }
        private static int Adet(Dictionary<CargoStatus, int> sayim, CargoStatus durum)
        {
            return sayim.TryGetValue(durum, out var adet) ? adet : 0;
        }

        public async Task<TopBranchesDto?> GetTopBranchesAsync(int take = 5)
        {
            var scope = await GetScopeAsync();

            if (!scope.IsUnrestricted)
            {
                return null;
            }

            var liste = await _cargoRepository.TopBranchesByTrafficScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted, take);

            var satirlar = liste.Select(x => new BranchTrafficDto
            {
                BranchName = x.BranchName,
                Count = x.Count
            }).ToList();

            return new TopBranchesDto
            {
                Rows = satirlar,

                // Hiç şube yoksa `Max()` boş listede patlar — görünüm de bu 0'ı boş duruma çeviriyor
                Max = satirlar.Count == 0 ? 0 : satirlar.Max(x => x.Count)
            };
        }

        public async Task<SlaSummaryDto?> GetSlaSummaryAsync()
        {
            var scope = await GetScopeAsync();

            if (!scope.IsUnrestricted)
            {
                return null;
            }

            var (teslim, zamaninda) = await _cargoRepository.CountOnTimeDeliveredScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted);

            return new SlaSummaryDto
            {
                Delivered = teslim,
                OnTime = zamaninda,
                Late = teslim - zamaninda,

                // Hiç teslim edilmiş kargo yoksa oran YOK: sıfıra bölme değil, anlamsız olduğu için
                OnTimeRate = teslim == 0 ? null : (double)zamaninda / teslim
            };
        }



        public async Task<ReasonBreakdownDto?> GetDelayReasonBreakdownAsync()
        {
            var scope = await GetScopeAsync();

            if (!scope.IsUnrestricted)
            {
                return null;
            }

            var sayim = await _cargoRepository.CountDelayReasonsScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted);

            return Dagilim(sayim, "Summary_DelayReasons", "DelayReason_");
        }

        public async Task<ReasonBreakdownDto?> GetFailureReasonBreakdownAsync()
        {
            var scope = await GetScopeAsync();

            if (!scope.IsUnrestricted)
            {
                return null;
            }

            var sayim = await _cargoRepository.CountFailureReasonsScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.CourierEmployeeId,
                scope.IsUnrestricted);

            return Dagilim(sayim, "Summary_FailureReasons", "DeliveryFailureReason_");
        }

        // İki dağılım birebir aynı şekilde kuruluyor: ikinci kopya yazmak yerine TEK yardımcı.
        // Neden listesi elle yazılmıyor, enum'dan geliyor — akışa bir neden eklenirse satır kendiliğinden çıkar
        private static ReasonBreakdownDto Dagilim<TEnum>(Dictionary<TEnum, int> sayim, string titleKey, string prefix)
            where TEnum : struct, Enum
        {
            var toplam = sayim.Values.Sum();

            // Beş nedenin beşi de satır alıyor, sıfırlar dahil: "hiç gümrük gecikmesi yok" da bir bilgi.
            // Sıra adet azalan, eşitlikte ADA göre — rastgele sıra olmasın
            var satirlar = Enum.GetValues<TEnum>()
                .Select(neden => new ReasonCountDto
                {
                    Reason = neden.ToString(),
                    Count = sayim.TryGetValue(neden, out var adet) ? adet : 0,
                    Share = toplam == 0 ? 0 : (double)(sayim.TryGetValue(neden, out var a2) ? a2 : 0) / toplam
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Reason)
                .ToList();

            return new ReasonBreakdownDto
            {
                TitleKey = titleKey,
                LabelPrefix = prefix,
                Total = toplam,
                Max = satirlar.Count == 0 ? 0 : satirlar.Max(x => x.Count),
                Rows = satirlar
            };
        }
        public async Task<List<RecentMovementDto>> GetRecentMovementsAsync(int count = 8)
        {
            var scope = await GetScopeAsync();

            // Günlük özetle aynı kapsam sırası: şube daha dar, varsa o kazanır
            List<CargoMovement> movements;

            // Şube adı yalnız ülke panelinde anlamlı — hangi kapsamdan geldiğimizi hatırlıyoruz
            bool ulkeKapsami = false;

            if (_currentUser.BranchId is not null)
            {
                movements = await _cargoMovementRepository.GetRecentByBranchAsync(_currentUser.BranchId.Value, count);
            }
            else if (scope.CountryId is not null)
            {
                movements = await _cargoMovementRepository.GetRecentByCountryAsync(scope.CountryId.Value, count);
                ulkeKapsami = true;
            }
            else
            {
                return [];
            }

            return movements.Select(m => new RecentMovementDto
            {
                CargoId = m.CargoId,
                TrackCode = m.Cargo.TrackCode,

                OldStatus = m.OldStatus,
                CargoStatus = m.CargoStatus,

                MovementDate = m.MovementDate,
                PerformedByName = m.PerformedByUser == null
                    ? null
                    : $"{m.PerformedByUser.FirstName} {m.PerformedByUser.LastName}",
                Description = m.Description,

                BranchName = ulkeKapsami ? m.Branch?.Name : null
            }).ToList();
        }

        // kapsam dışındaki kayda dokunulamaz — kargo var ama bu kullanıcının değil
        private async Task<Cargo> FindInScopeAsync(Guid cargoId)
        {
            var scope = await GetScopeAsync();

            var cargo = await _cargoRepository.GetScopedAsync(cargoId, scope.BranchId, scope.CountryId,
                                                              scope.CustomerId, scope.UserId, scope.CourierEmployeeId, scope.IsUnrestricted);
            if (cargo != null)
            {
                return cargo;
            }

            // Kargo gerçekten yok mu, yoksa kapsam dışında mı — iki durum ayrı mesaj alır
            var exists = await _cargoRepository.GetByIdAsync(cargoId) != null;
            throw new ValidationException(_localizer[exists ? "UnauthorizedRecord" : "CargoNotFound"].Value);
        }

    }
}
