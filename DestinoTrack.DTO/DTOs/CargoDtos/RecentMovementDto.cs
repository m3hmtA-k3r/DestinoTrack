using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // VC9'un bir satırı: şubede en son ne oldu
    public class RecentMovementDto
    {
        public Guid CargoId { get; set; }
        public string TrackCode { get; set; }

        // Geçiş: eski durum boşsa kargo yeni doğmuş demektir
        public CargoStatus? OldStatus { get; set; }
        public CargoStatus CargoStatus { get; set; }

        public DateTime MovementDate { get; set; }
        public string? PerformedByName { get; set; }
        public string? Description { get; set; }
    }
}
