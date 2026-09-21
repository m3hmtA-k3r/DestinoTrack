namespace DestinoTrack.DTO.DTOs.CityDtos
{
    // Formlardaki şehir açılır listesi için sade kayıt — sayfalama yok, bütün şehirler gelir
    public class CityLookupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Ülkeye göre süzme için: form önce ülke sorar, sonra o ülkenin şehirlerini gösterir
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
