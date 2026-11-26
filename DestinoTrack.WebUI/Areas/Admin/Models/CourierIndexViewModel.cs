using DestinoTrack.DTO.DTOs.BranchDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    public class CourierIndexViewModel
    {
        public PagedResult<CourierOverviewDto> Couriers { get; set; } = new();
        public List<BranchLookupDto> Branches { get; set; } = new();

        public Guid? SelectedBranchId { get; set; }
        public VehicleType? SelectedVehicleType { get; set; }
        public bool? SelectedIsActive { get; set; }
        public string? Search { get; set; }

        public bool IsFiltered => SelectedBranchId.HasValue
                                  || SelectedVehicleType.HasValue
                                  || SelectedIsActive.HasValue
                                  || !string.IsNullOrWhiteSpace(Search);
    }
}
