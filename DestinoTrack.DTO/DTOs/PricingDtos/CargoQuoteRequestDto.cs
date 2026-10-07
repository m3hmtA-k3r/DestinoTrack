using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.PricingDtos
{
    // Fiyat sorusu: kargo kaydı açılmadan da sorulabilir
    public class CargoQuoteRequestDto
    {
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }

        public CargoType CargoType { get; set; } = CargoType.Standard;

        public double Weight { get; set; }   // kg

        // Desi buradan hesaplanır: (En × Boy × Yükseklik) / 3000 
        public double Width { get; set; }    // cm
        public double Height { get; set; }   // cm
        public double Length { get; set; }   // cm

        // Boş bırakılırsa "bugün" sayılır; tahmini teslim bunun üzerine eklenir
        public DateTime? ShipmentDate { get; set; }
    }
}
