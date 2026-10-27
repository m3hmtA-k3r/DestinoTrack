using DestinoTrack.DTO.DTOs.CargoDtos;

namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    // Yeni hareket ekranı iki adımı tek görünümde gösteriyor:
    // Target boşsa birinci adım (yalnız takip no kutusu), doluysa ikinci adım (durum + açıklama)
    public class MovementViewModel
    {
        // Birinci adımda personelin yazdığı numara; ikinci adımda gizli alanla taşınıyor
        public string? TrackCode { get; set; }

        // Birinci adımın sonucu: kargo ve durum makinesinin o an izin verdiği durumlar.
        // Boş olması "henüz aranmadı" demek — iki hâli ayıran tek işaret (kabul ekranındaki Quote gibi)
        public CargoMovementTargetDto? Target { get; set; }

        // Doğrulama kuralları CreateMovementValidator'da — ekran kendi kuralını yazmıyor
        public CreateMovementDto Movement { get; set; } = new();
    }
}
