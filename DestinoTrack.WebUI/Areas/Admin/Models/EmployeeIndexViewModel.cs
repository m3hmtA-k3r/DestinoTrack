using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    // liste + görev çipleri + o an seçili filtre
    public class EmployeeIndexViewModel
    {
        public PagedResult<ResultEmployeeDto> Employees { get; set; } = new();
        public List<EmployeeJobTypeFilterDto> JobTypeFilters { get; set; } = new();

        public EmployeeJobType? SelectedJobType { get; set; }
        public string? Search { get; set; }

        // "Tümü" sayısı: filtreden bağımsız, bütün personel
        public int TotalEmployeeCount => JobTypeFilters.Sum(f => f.EmployeeCount);

        public bool IsFiltered => SelectedJobType.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
