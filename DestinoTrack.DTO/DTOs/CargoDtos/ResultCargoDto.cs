using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Liste satırı — kapsam süzgecinden geçmiş kargolar
    public class ResultCargoDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public CargoType CargoType { get; set; }

        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string OriginBranchName { get; set; }
        public string DestinationBranchName { get; set; }

        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsPaid { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
    }
}
