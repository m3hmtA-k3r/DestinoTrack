using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.VehicleDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.Vehicles
{
    public interface IVehicleService
    {
        Task<PagedResult<ResultVehicleDto>> GetPagedAsync(Guid? branchId = null, VehicleType? vehicleType = null,
                                                          bool? isActive = null, string? search = null, int page = 1);

        // branchId null → bütün şubelerin aktif araçları 
        Task<List<VehicleLookupDto>> GetLookupAsync(Guid? branchId = null);

        Task<UpdateVehicleDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateVehicleDto createVehicleDto);
        Task UpdateAsync(UpdateVehicleDto updateVehicleDto);
        Task DeleteAsync(Guid id);
    }
}
