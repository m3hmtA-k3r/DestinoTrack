using DestinoTrack.Entity.Entities.Common;

namespace DestinoTrack.Entity.Entities
{
    public class Country : BaseEntity // TR · MT · BR | TRY · EUR · BRL | +90 · +356 · +55 | Europe/Istanbul · Europe/Malta · America/Sao_Paulo | tr · en · pt
    {
        public string Name { get; set; }
        public string IsoCode { get; set; }        
        public string CurrencyCode { get; set; }   
        public string PhoneCode { get; set; }      
        public string TimeZoneId { get; set; }      
        public string LanguageCode { get; set; }    

        // Navigation Property
        public virtual IList<City> Cities { get; set; }
    }
}
