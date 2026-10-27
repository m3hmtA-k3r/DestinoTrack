using DestinoTrack.DTO.DTOs.BranchDtos;
using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.PricingDtos;

namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    // Kargo kabul ekranı: form + varış şubesi listesi + (sorulduysa) fiyat önizlemesi
    public class CargoCreateViewModel
    {
        // Doğrulama kuralları CreateCargoValidator'da (TALP-63 e baglı yaptıgım bu detay) — ekran kendi kuralını yazmıyor
        public CreateCargoDto Cargo { get; set; }

        //çıkış şubesi seçilmez — personelin kendi şubesi, formda yalnız gösterilir
        public string OriginBranchName { get; set; }

        public List<BranchLookupDto> DestinationBranches { get; set; } 

        // "Fiyatı Hesapla" basıldıysa dolu, boş formda null — iki hâli ayıran tek işaret
        public CargoQuoteDto? Quote { get; set; }
    }
}
