using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Yeni hareket ekranının ikinci adımı: hangi kargo, hangi duruma, açıklamasıyla.
    // Şube ve işlemi yapan kullanıcı burada YOK — onları sunucu yazıyor; formdan gelen
    // bir alan olmadıkları için doğrulayıcı da onları zorunlu tutmuyor (TALP-83 dersi)
    public class CreateMovementDto
    {
        public Guid CargoId { get; set; }
        public CargoStatus NewStatus { get; set; }

        // İsteğe bağlı: "Alıcı adreste bulunamadı", "Araç arızası" gibi
        public string? Description { get; set; }
    }
}
