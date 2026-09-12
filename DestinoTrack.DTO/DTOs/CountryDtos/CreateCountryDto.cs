namespace DestinoTrack.DTO.DTOs.CountryDtos
{
    public class CreateCountryDto
    {
        public string Name { get; set; }
        public string IsoCode { get; set; }
        public string CurrencyCode { get; set; }
        public string PhoneCode { get; set; }
        public string TimeZoneId { get; set; }
        public string LanguageCode { get; set; }
    }
}
