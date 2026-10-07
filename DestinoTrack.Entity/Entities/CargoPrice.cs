using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    // Fiyat tarifesi: çıkış ülkesi × rota kademesi başına tek satır.
    // Ücret = (BasePrice + max(Ağırlık, Desi) × PricePerDesi) × kargo tipi çarpanı
    public class CargoPrice : BaseEntity
    {
        // Çıkış ülkesi — para birimi bu ülkeden gelir (Country.CurrencyCode), tarifede ayrıca tutulmaz
        public Guid CountryId { get; set; }

        public RouteScope RouteScope { get; set; }

        // Gönderi başına sabit ücret
        public decimal BasePrice { get; set; }

        // Ücretlendirilecek birim (ağırlık ile desinin büyüğü) başına ücret
        public decimal PricePerDesi { get; set; }

        //  tahmini teslim = gönderi tarihi + bu gün sayısı (+ kargo tipinin gün farkı)
        public int TransitDays { get; set; }

        // Navigation Properties
        public virtual Country Country { get; set; }
    }
}
