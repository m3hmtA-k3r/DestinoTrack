using DestinoTrack.Business.Services.Pricing;
using DestinoTrack.DataAccess.Repositories.CargoMovements;
using DestinoTrack.DataAccess.Repositories.Cargos;
using DestinoTrack.DataAccess.Repositories.Customers;
using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Cargos
{
    public class CargoService(ICargoRepository _cargoRepository,
                          ICargoMovementRepository _cargoMovementRepository,
                          ICustomerRepository _customerRepository,
                          ICargoPricingService _cargoPricingService,
                          ITrackCodeGenerator _trackCodeGenerator,
                          IStringLocalizer<SharedResource> _localizer) : ICargoService

    {
        // izin verilen geçisler Kural kodda durur, veritabanından deghiştirilemez.
        // Listede olmayan her geçiş yasaktır — Teslim Edildi ve İade Edildi son durumlardır.
        private static readonly IReadOnlyDictionary<CargoStatus, CargoStatus[]> AllowedTransitions =
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
            };


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

                // D40: bilgi kargoda, hesap bağı isteğe bağlı
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

            // L3: ilk hareket — önceki durum yok, kargo çıkış şubesinde doğuyor
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


        public bool CanTransition(CargoStatus from, CargoStatus to)
        {
            return AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
        }

        public async Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid branchId, Guid performedByUserId,
                                            string? description = null, DelayReason? delayReason = null)
        {
            var cargo = await _cargoRepository.GetByIdAsync(cargoId)
                        ?? throw new ValidationException(_localizer["CargoNotFound"].Value);

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


    }
}
