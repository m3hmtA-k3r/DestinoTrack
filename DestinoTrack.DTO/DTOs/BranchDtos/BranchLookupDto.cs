namespace DestinoTrack.DTO.DTOs.BranchDtos
{
    // Formlardaki tesis açılır listesi: sayfasız, ülkeye göre süzülür (data-parent)
    public class BranchLookupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
    }
}
