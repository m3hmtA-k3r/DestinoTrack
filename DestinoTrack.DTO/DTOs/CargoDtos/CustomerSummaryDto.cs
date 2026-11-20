using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    public class CustomerSummaryDto
    {
        public int Active { get; set; }

        public int Delivered { get; set; }
        public string? LatestTrackCode { get; set; }
        public Guid? LatestId { get; set; }
        public CargoStatus? LatestStatus { get; set; }
        public DateTime? LatestMovementDate { get; set; }
    }
}
