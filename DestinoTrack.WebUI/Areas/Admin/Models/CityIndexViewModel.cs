using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.DTO.DTOs.Common;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    
    // liste + ülke  + o an seçili filtre
    public class CityIndexViewModel
    {
        public PagedResult<ResultCityDto> Cities { get; set; } = new();
        public List<CityCountryFilterDto> CountryFilters { get; set; } = new();


        public Guid? SelectedCountryId { get; set; }
        public string? Search { get; set; }

        // "Tümü"  sayı: filtreden bağımsız, bütün şehirler
        public int TotalCityCount => CountryFilters.Sum(c => c.CityCount);

        // Liste boşsa "hiç kayıt yok" ile "filtreye uyan yok" ayrımı için
        public bool IsFiltered => SelectedCountryId.HasValue || !string.IsNullOrWhiteSpace(Search);
    }
}
