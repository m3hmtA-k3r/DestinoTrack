namespace DestinoTrack.DTO.DTOs.CityDtos
{
    public class UpdateCityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
    }
}
