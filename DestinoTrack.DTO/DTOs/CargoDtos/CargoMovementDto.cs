using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Hareket çizelgesinin bir satırı — girişsiz takip ve müşteri paneli de kullanacak
    public class CargoMovementDto
    {
        public DateTime MovementDate { get; set; }

        public CargoStatus? OldStatus { get; set; }   // ilk harekette boş
        public CargoStatus CargoStatus { get; set; }

        public string? Description { get; set; }
        public DelayReason? DelayReason { get; set; }

        public string BranchName { get; set; }
        public string PerformedByName { get; set; }
    }
}
