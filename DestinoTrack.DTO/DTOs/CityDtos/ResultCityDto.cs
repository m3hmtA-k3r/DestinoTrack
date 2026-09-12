namespace DestinoTrack.DTO.DTOs.CityDtos
{
    public class ResultCityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
