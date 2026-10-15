using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Girişsiz takip sayfasındaki tek hareket satırı.
    public class PublicMovementDto
    {
        public DateTime MovementDate { get; set; }
        public CargoStatus CargoStatus { get; set; }

        public string BranchName { get; set; }
        public string CityName { get; set; }

        // serbest metin değil, tanımlı sebep — alıcının görmesi işine yarar
        public DelayReason? DelayReason { get; set; }
    }
}
