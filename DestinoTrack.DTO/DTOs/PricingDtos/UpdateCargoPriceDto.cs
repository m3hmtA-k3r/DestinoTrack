using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.PricingDtos
{
    public class UpdateCargoPriceDto
    {
        public Guid Id { get; set; }

        public Guid CountryId { get; set; }
        public RouteScope RouteScope { get; set; }

        public decimal BasePrice { get; set; }
        public decimal PricePerDesi { get; set; }
        public int TransitDays { get; set; }
    }
}
