using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CustomerDtos
{
    // Admin'in elle açtığı müşteri. Kod serviste üretilir (D114), durum Active doğar (D115):
    // ikisi de formda yok. Kredi limiti ve sözleşme tarihleri de yok (D116) — onları okuyan kod henüz yok
    public class CreateCustomerDto
    {
        // Kurumsalda ünvan, bireyselde ad soyad
        public string Title { get; set; }

        // Vergi numarasının biçimini bu ikisi birlikte belirler: TaxIdentityRules ülke + tür bakıyor
        public CustomerType CustomerType { get; set; }
        public Guid CountryId { get; set; }

        public string TaxNumber { get; set; }
        public string TaxOffice { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
