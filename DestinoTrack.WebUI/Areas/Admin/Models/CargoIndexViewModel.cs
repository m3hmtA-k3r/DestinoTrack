using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    // Tüm kargolar ekranı: liste + durum çipleri + o an seçili filtre
    public class CargoIndexViewModel
    {
        public PagedResult<ResultCargoDto> Cargos { get; set; } = new();

        public CargoStatus? SelectedStatus { get; set; }
        public string? Search { get; set; }

        public bool IsFiltered => SelectedStatus.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
