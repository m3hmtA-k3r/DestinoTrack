using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.PricingDtos
{
    // Fiyat cevabı: tutar + tahmini teslim + hesabın nasıl çıktığı
    public class CargoQuoteDto
    {
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }  

        public DateTime EstimatedArrivalDate { get; set; }
        public int TransitDays { get; set; }     

        // Hesabın açıklaması — ekranda "neye göre" diye gösterilebilir
        public RouteScope RouteScope { get; set; }
        public double Desi { get; set; }
        public decimal ChargeableUnit { get; set; }   // max(ağırlık, desi)
        public decimal BasePrice { get; set; }
        public decimal PricePerDesi { get; set; }
        public decimal TypeMultiplier { get; set; }
    }
}
