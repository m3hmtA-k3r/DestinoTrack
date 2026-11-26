using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class CustomerMovementDto
    {
        public DateTime MovementDate { get; set; }
        public CargoStatus CargoStatus { get; set; }

        public string? BranchName { get; set; }
        public string? CityName { get; set; }

        // Serbest metin değil, tanımlı sebep — müşterinin görmesi işine yarar
        public DelayReason? DelayReason { get; set; }
    }
}
