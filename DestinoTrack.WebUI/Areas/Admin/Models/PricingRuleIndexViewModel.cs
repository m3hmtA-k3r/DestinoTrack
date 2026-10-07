using DestinoTrack.DTO.DTOs.PricingDtos;

namespace DestinoTrack.WebUI.Areas.Admin.Models
{
    // Fiyat kuralları ekranı: üstte tarifeler, altta sekiz tip çarpanı
    public class PricingRuleIndexViewModel
    {
        public List<ResultCargoPriceDto> Tariffs { get; set; } = new();
        public List<CargoTypeRateDto> Rates { get; set; } = new();
    }
}
