using DestinoTrack.DataAccess.Repositories.Branches;
using DestinoTrack.DataAccess.Repositories.Cargos;
using DestinoTrack.DataAccess.Repositories.Cities;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DTO.DTOs.DashboardDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.Dashboard
{
    public class DashboardService(
        ICargoRepository _cargoRepository,
        ICountryRepository _countryRepository,
        ICityRepository _cityRepository,
        IBranchRepository _branchRepository
        ) : IDashboardService
    {
        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            return new DashboardSummaryDto
            {
                TotalCargoCount = await _cargoRepository.CountAsync(),
                OutForDeliveryCount = await _cargoRepository.CountByStatusAsync(CargoStatus.OutForDelivery),
                DeliveredCount = await _cargoRepository.CountByStatusAsync(CargoStatus.Delivered),
                InTransferCenterCount = await _cargoRepository.CountByStatusAsync(CargoStatus.InTransferCenter),

                CountryCount = await _countryRepository.CountAsync(),
                CityCount = await _cityRepository.CountAsync(),
                BranchCount = await _branchRepository.CountAsync()
            };
        }
    }
}
