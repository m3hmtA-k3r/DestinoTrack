using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class RecentMovementDto
    {
        public Guid CargoId { get; set; }
        public string TrackCode { get; set; }

        public CargoStatus? OldStatus { get; set; }
        public CargoStatus CargoStatus { get; set; }

        public DateTime MovementDate { get; set; }
        public string? PerformedByName { get; set; }
        public string? BranchName { get; set; }
        public string? Description { get; set; }
    }
}
