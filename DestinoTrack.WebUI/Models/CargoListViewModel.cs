using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.WebUI.Models
{
    // Üç panelin ortak kargo listesi modeli: Admin (tüm kargolar) · Personel (şube) · Manager (ülke).
    // Kapsamı servis uyguluyor (GetScopeAsync), model yalnız o anki filtreyi taşır.
    public class CargoListViewModel
    {
        public PagedResult<ResultCargoDto> Cargos { get; set; } = new();
        
        // Yön yalnız şube kapsamında anlamlı: CargoRepository.Scoped ülke dalında yönü
        // kullanmadan dönüyor, Admin'in ise kapsamı yok. O iki panelde null kalır.

        public CargoDirection? SelectedDirection { get; set; }

        public CargoStatus? SelectedStatus { get; set; }
        public string? Search { get; set; }

        public bool IsFiltered => SelectedDirection.HasValue || SelectedStatus.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
