using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CargoDtos
{
    // Burada olmayan bir alan sayfaya çıkamaz: koruma dikkate değil, sözleşmeye bağlı
    public class PublicTrackingDto
    {
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public CargoType CargoType { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // Güzergah şehir düzeyinde; şube adı yalnız çizelgede  
        public string OriginCityName { get; set; }
        public string DestinationCityName { get; set; }

        //yalnız alıcı ödemeli ve henüz ödenmemiş gönderide dolu — diğer hallerde null
        public decimal? AmountDueOnDelivery { get; set; }
        public string? CurrencyCode { get; set; }

        public List<PublicMovementDto> Movements { get; set; } = new();
    }
}
