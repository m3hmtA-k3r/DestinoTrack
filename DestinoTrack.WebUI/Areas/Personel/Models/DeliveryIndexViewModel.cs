using DestinoTrack.DTO.DTOs.CargoDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.EmployeeDtos;

namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    // Dağıtım ekranı tek sayfada iki bölüm : üstte dağıtıma hazır olanlar, altta dağıtımdakiler
    public class DeliveryIndexViewModel
    {
        // Üst bölüm yalnız Personel'e çizilir. Kurye kendine kargo atamaz: hangi kargoyu kimin
        // taşıyacağına şube karar verir — kuryede bu liste null kalır
        public PagedResult<DeliveryRowDto>? ReadyForDispatch { get; set; }

        // Dağıtıma çıkarma formundaki seçim: şubenin aktif kuryeleri
        public List<EmployeeLookupDto> Couriers { get; set; } = [];

        // Alt bölüm: kuryenin üzerindeki kargolar, satır içi teslim / teslim edilemedi 
        public PagedResult<DeliveryRowDto> OnDelivery { get; set; }

        // Üst bölüm çizilecek mi — görünüm rolü kendisi sorgulamasın, karar controller'da verilmiş olsun
        public bool CanDispatch => ReadyForDispatch is not null;
    }
}
