using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.PricingDtos
{
    // Fiyat kuralları ekranındaki tarife satırı
    public class ResultCargoPriceDto
    {
        public Guid Id { get; set; }

        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public string CurrencyCode { get; set; }   // Country'den 

        public RouteScope RouteScope { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerDesi { get; set; }
        public int TransitDays { get; set; }
    }
}
