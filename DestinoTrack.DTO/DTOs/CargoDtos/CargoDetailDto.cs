using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Admin kargo detayı : bilgiler + teslimat + hareket çizelgesi
    public class CargoDetailDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public string Barcode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public CargoType CargoType { get; set; }

        // Taraflar (bilgi kargoda)
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public string? CustomerTitle { get; set; }     // kurumsal gönderide dolu

        // Güzergah ve ölçüler
        public string OriginBranchName { get; set; }
        public string DestinationBranchName { get; set; }
        public double Weight { get; set; }
        public double Desi { get; set; }

        // Ücret ve tarihler
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsPaid { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }

        // Dağıtım ve teslimat  
        public string? CourierName { get; set; }
        public string? DeliveryCode { get; set; }           // dağıtıma çıkmışsa
        public string? RecipientName { get; set; }          // teslim edildiyse
        public DateTime? DeliveredAt { get; set; }
        public int FailedAttemptCount { get; set; }

        // zaman çizelgesi, eskiden yeniye
        public List<CargoMovementDto> Movements { get; set; } = new();
    }
}
