using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.PricingDtos
{
    public class CreateCargoPriceDto
    {
        public Guid CountryId { get; set; }
        public RouteScope RouteScope { get; set; } = RouteScope.SameCity;

        public decimal BasePrice { get; set; }
        public decimal PricePerDesi { get; set; }

        // Kademenin teslim süresi (gün) — tip gün farkı bunun üzerine eklenir
        public int TransitDays { get; set; } = 1;
    }
}
