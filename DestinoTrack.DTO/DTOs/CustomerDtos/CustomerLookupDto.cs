namespace DestinoTrack.DTO.DTOs.CustomerDtos
{
    // Gişe formundaki müşteri açılır listesi: sayfasız, yalnız Active müşteriler.
    // Kod da gösteriliyor çünkü aynı ünvanda iki müşteri olabilir
    public class CustomerLookupDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
