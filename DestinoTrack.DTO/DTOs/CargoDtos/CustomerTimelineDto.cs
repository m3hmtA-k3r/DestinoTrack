using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // EN SON HAREKET GÖREN kargonun yaşam öyküsü + tahmini teslim. Kargo yoksa servis null döner, bileşen sessiz   
    public class CustomerTimelineDto
    {
        public string TrackCode { get; set; } = string.Empty;
        public CargoStatus CargoStatus { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public List<CustomerMovementDto> Movements { get; set; } = new();
    }
}
