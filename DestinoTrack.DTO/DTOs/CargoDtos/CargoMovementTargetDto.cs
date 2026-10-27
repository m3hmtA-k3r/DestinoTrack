using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Yeni hareket ekranının birinci adımının sonucu: takip numarasıyla bulunan kargo ve
    // durum makinesinin o an izin verdiği durumlar. Serbest durumlar burada geliyor,
    // görünümde elle yazılmıyor — geçiş haritası tek yerde kalsın
    public class CargoMovementTargetDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }

        public CargoStatus CurrentStatus { get; set; }
        public List<CargoStatus> AllowedStatuses { get; set; } = [];

        // Personel doğru kargoya hareket yazdığından emin olsun diye
        public string OriginBranchName { get; set; }
        public string DestinationBranchName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime? LastMovementDate { get; set; }

        // kendi ekranı mı var (kurye ataması · teslim · iade)? Ekran doğru şeyi söylesin diye
        public bool HandledInDeliveryScreen { get; set; }

    }
}
