using DestinoTrack.DTO.DTOs.AuditDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    public class AuditLogIndexViewModel
    {
        public PagedResult<AuditLogRowDto> Logs { get; set; } = new();

        // Kayıtta gerçekten geçen tür adları (City, Branch, Cargo...) — sabit liste değil
        public List<string> EntityFilters { get; set; } = new();

        public AuditAction? SelectedAction { get; set; }
        public string? SelectedEntityName { get; set; }
        public string? Search { get; set; }

        public bool IsFiltered => SelectedAction.HasValue
                                  || !string.IsNullOrWhiteSpace(SelectedEntityName)
                                  || !string.IsNullOrWhiteSpace(Search);
    }
}
