using DestinoTrack.Business.Consts;
using DestinoTrack.DataAccess.Repositories.Vehicles;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.VehicleDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Mapster;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Vehicles
{
    public class VehicleService(IVehicleRepository _vehicleRepository, IStringLocalizer<SharedResource> _localizer) : IVehicleService
    {
        public async Task<PagedResult<ResultVehicleDto>> GetPagedAsync(Guid? branchId = null, VehicleType? vehicleType = null,
                                                                       bool? isActive = null, string? search = null, int page = 1)
        {
            // Adres çubuğuna ?page=0 yazılabilir: en küçük sayfa 1
            page = page < 1 ? 1 : page;

            var (vehicles, totalCount) = await _vehicleRepository.GetPagedAsync(branchId, vehicleType, isActive, search, page, Paging.PageSize);

            // Son sayfadaki tek kayıt silinince o sayfa boş kalır → varsa yeni son sayfa gösterilir
            if (vehicles.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (vehicles, totalCount) = await _vehicleRepository.GetPagedAsync(branchId, vehicleType, isActive, search, page, Paging.PageSize);
            }

            // Kurye sayıları TEK sorguda: lazy loading açık olduğu için v.Couriers.Count yazmak N+1 üretirdi
            var courierCounts = await _vehicleRepository.GetCourierCountsAsync();

            return new PagedResult<ResultVehicleDto>
            {
                Items = vehicles.Select(v => new ResultVehicleDto
                {
                    Id = v.Id,
                    Plate = v.Plate,
                    VehicleType = v.VehicleType,
                    BranchId = v.BranchId,
                    BranchName = v.Branch?.Name,
                    IsActive = v.IsActive,
                    CourierCount = courierCounts.GetValueOrDefault(v.Id)
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<VehicleLookupDto>> GetLookupAsync(Guid? branchId = null)
        {
            var vehicles = await _vehicleRepository.GetLookupAsync(branchId);

            return vehicles.Select(v => new VehicleLookupDto
            {
                Id = v.Id,
                Plate = v.Plate,
                VehicleType = v.VehicleType,
                BranchId = v.BranchId
            }).ToList();
        }

        public async Task<UpdateVehicleDto> GetByIdAsync(Guid id)
        {
            var vehicle = await FindAsync(id);
            return vehicle.Adapt<UpdateVehicleDto>();
        }

        public async Task CreateAsync(CreateVehicleDto createVehicleDto)
        {
            var vehicle = createVehicleDto.Adapt<Vehicle>();
            vehicle.Plate = NormalizePlate(createVehicleDto.Plate);

            await EnsurePlateIsFreeAsync(vehicle.Plate, null);

            await _vehicleRepository.CreateAsync(vehicle);
        }

        public async Task UpdateAsync(UpdateVehicleDto updateVehicleDto)
        {
            var vehicle = await FindAsync(updateVehicleDto.Id);

            // Mevcut kayıt üzerine yazılır: CreatedDate korunur (DTO'da yok)
            updateVehicleDto.Adapt(vehicle);
            vehicle.Plate = NormalizePlate(updateVehicleDto.Plate);

            await EnsurePlateIsFreeAsync(vehicle.Plate, vehicle.Id);

            await _vehicleRepository.UpdateAsync(vehicle);
        }

        public async Task DeleteAsync(Guid id)
        {
            var vehicle = await FindAsync(id);

            // Kuryesi olan araç silinmez — soft delete'te bunu veritabanı engellemiyor
            if (await _vehicleRepository.HasCouriersAsync(id))
            {
                throw new ValidationException(_localizer["VehicleHasCouriers"].Value);
            }

            await _vehicleRepository.DeleteAsync(vehicle);
        }

        private async Task<Vehicle> FindAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                throw new ValidationException(_localizer["VehicleNotFound"].Value);
            }

            return vehicle;
        }

        // ToUpperInvariant bilerek: tr-TR kültüründe ToUpper() "i" harfini "İ" yapar ve plaka bozulur.
        // Normalleştirme TEK yerde; veritabanındaki tekil indeks de büyük harfli plaka bekliyor
        private static string NormalizePlate(string plate)
        {
            return (plate ?? string.Empty).Trim().ToUpperInvariant();
        }

        private async Task EnsurePlateIsFreeAsync(string plate, Guid? excludeId)
        {
            if (await _vehicleRepository.PlateExistsAsync(plate, excludeId))
            {
                throw new ValidationException(_localizer["VehiclePlateInUse"].Value);
            }
        }
    }
}
