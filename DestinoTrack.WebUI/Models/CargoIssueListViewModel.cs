using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Models
{
    // Sorunlu kargolar: tek liste + küme çipleri (başarısız denemeli · iade sürecinde · geciken).
    // Şube ve ülke paneli aynı modeli kullanıyor; kapsamı servis seçiyor
    public class CargoIssueListViewModel
    {
        public PagedResult<CargoIssueRowDto> Cargos { get; set; } = new();

        public CargoIssue? SelectedIssue { get; set; }

        public DateTime Now { get; set; } = DateTime.UtcNow;
    }
}
