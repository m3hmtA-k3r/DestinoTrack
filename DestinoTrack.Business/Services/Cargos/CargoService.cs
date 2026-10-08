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
using System.Collections.Frozen;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

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
                IsPaid = false,

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

        public async Task<PagedResult<ResultCargoDto>> GetPagedAsync(CargoStatus? status = null, string? search = null, int page = 1)
        {
            page = page < 1 ? 1 : page;
            var scope = GetScope();

            var (cargos, totalCount) = await _cargoRepository.GetPagedScopedAsync(
                scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.IsUnrestricted,
                status, search, page, Paging.PageSize);

            // Son sayfadaki tek kayıt silinince o sayfa boş kalır → varsa yeni son sayfa gösterilir
            if (cargos.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (cargos, totalCount) = await _cargoRepository.GetPagedScopedAsync(
                    scope.BranchId, scope.CountryId, scope.CustomerId, scope.UserId, scope.IsUnrestricted,
                    status, search, page, Paging.PageSize);
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


        public bool CanTransition(CargoStatus from, CargoStatus to)
        {
            return AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
        }

        public async Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid branchId, Guid performedByUserId,
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
            await ChangeStatusAsync(cargoId, CargoStatus.OutForDelivery, branchId, performedByUserId,
                                    _localizer["CargoDispatchedMovement", $"{courier.FirstName} {courier.LastName}"].Value);

            return cargo.DeliveryCode;
        }

        public async Task DeliverAsync(Guid cargoId, string deliveryCode, string recipientName, Guid courierEmployeeId,
                               Guid branchId, Guid performedByUserId)
        {
            var cargo = await FindInScopeAsync(cargoId);

            //kargo kime atandıysa teslimi o yapar — başka kurye kapatamaz
            if (cargo.CourierId != courierEmployeeId)
            {
                throw new ValidationException(_localizer["CargoNotAssignedToCourier"].Value);
            }

            //  kod doğrulanmadan teslim yok. Boşluklar temizlenir ama hane hane aynı olmalı
            if (string.IsNullOrWhiteSpace(cargo.DeliveryCode) || cargo.DeliveryCode != deliveryCode?.Trim())
            {
                throw new ValidationException(_localizer["DeliveryCodeInvalid"].Value);
            }

            if (string.IsNullOrWhiteSpace(recipientName))
            {
                throw new ValidationException(_localizer["RecipientNameRequired"].Value);
            }

            // Önce durum geçişi: Dağıtıma Çıktı değilse zaten buraya gelinemez
            await ChangeStatusAsync(cargoId, CargoStatus.Delivered, branchId, performedByUserId,
                                    _localizer["CargoDeliveredMovement", recipientName.Trim()].Value);

            //teslimin kanıtı — hangi kodla, kime, kim tarafından, ne zaman
            await _deliveryRepository.CreateAsync(new Delivery
            {
                CargoId = cargo.Id,
                Code = cargo.DeliveryCode,
                RecipientName = recipientName.Trim(),
                DeliveredByEmployeeId = courierEmployeeId,
                DeliveredAt = DateTime.UtcNow
            });
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
            await ChangeStatusAsync(cargoId, CargoStatus.DeliveryFailed, branchId, performedByUserId,
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
                await ChangeStatusAsync(cargoId, CargoStatus.ReturnInProgress, branchId, performedByUserId,
                                        _localizer["ReturnStartedMovement", attemptNo].Value);
            }

        }

        // kapsam roldan ve kullanıcının şube/ülke/müşteri bilgisinden çıkar
        private CargoScope GetScope()
        {
            return _currentUser.Role switch
            {
                RoleNames.Admin => new CargoScope { IsUnrestricted = true },
                RoleNames.Manager => new CargoScope { CountryId = _currentUser.CountryId },
                RoleNames.Personel or RoleNames.Courier => new CargoScope { BranchId = _currentUser.BranchId },

                // Kurumsal çalışanı şirketinin kargolarını, bireysel müşteri kendi gönderdiklerini görür
                RoleNames.Customer => new CargoScope
                {
                    CustomerId = _currentUser.CustomerId,
                    UserId = _currentUser.CustomerId is null ? _currentUser.UserId : null
                },

                // Tanımsız rol ya da girişsiz istek: hiçbir şey görmez
                _ => new CargoScope()
            };
        }

        // D49: kapsam dışındaki kayda dokunulamaz — kargo var ama bu kullanıcının değil
        private async Task<Cargo> FindInScopeAsync(Guid cargoId)
        {
            var scope = GetScope();

            var cargo = await _cargoRepository.GetScopedAsync(cargoId, scope.BranchId, scope.CountryId,
                                                              scope.CustomerId, scope.UserId, scope.IsUnrestricted);
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
