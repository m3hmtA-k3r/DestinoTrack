using DestinoTrack.DTO.DTOs.BranchDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    // liste + tür çipleri + o an seçili filtre
    public class BranchIndexViewModel
    {
        public PagedResult<ResultBranchDto> Branches { get; set; } = new();
        public List<BranchTypeFilterDto> TypeFilters { get; set; } = new();

        public BranchType? SelectedType { get; set; }
        public string? Search { get; set; }

        // "Tümü" sayısı: filtreden bağımsız, bütün tesisler
        public int TotalBranchCount => TypeFilters.Sum(t => t.BranchCount);

        // Liste boşsa "hiç kayıt yok" ile "filtreye uyan yok" ayrımı için
        public bool IsFiltered => SelectedType.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
