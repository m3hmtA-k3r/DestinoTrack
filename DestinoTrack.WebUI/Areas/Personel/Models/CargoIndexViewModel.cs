using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    // Şube kargoları ekranı: liste + yön (gelen / çıkan) + durum çipleri + arama
    public class CargoIndexViewModel
    {
        public PagedResult<ResultCargoDto> Cargos { get; set; } = new();

        public CargoDirection? SelectedDirection { get; set; }
        public CargoStatus? SelectedStatus { get; set; }
        public string? Search { get; set; }

        public bool IsFiltered => SelectedDirection.HasValue || SelectedStatus.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
