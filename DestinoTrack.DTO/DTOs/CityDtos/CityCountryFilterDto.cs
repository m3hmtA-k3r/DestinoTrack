namespace DestinoTrack.DTO.DTOs.CityDtos
{
    // Şehir listesinin üstündeki ülke çipi: [Türkiye 81]
    public class CityCountryFilterDto
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public int CityCount { get; set; }
    }
}
