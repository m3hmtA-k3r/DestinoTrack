using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    // Sorunlu kargolar ekranı: tek liste + küme çipleri (başarısız denemeli · iade sürecinde · geciken)
    public class CargoIssueIndexViewModel
    {
        public PagedResult<CargoIssueRowDto> Cargos { get; set; } = new();

        // Çip seçilmemişse üç kümenin birleşimi gösterilir
        public CargoIssue? SelectedIssue { get; set; }

        // Satırın gecikip gecikmediği görünümde yeniden hesaplanmasın: kural tek yerde.
        // Liste zaten kapsam + küme süzgecinden geçti, burada yalnız rozet boyanıyor
        public DateTime Now { get; set; } = DateTime.UtcNow;
    }
}
