using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    public class RegisterDto
    {
        //Bireysel → hesap Aktif · Kurumsal → Onay bekliyor
        public CustomerType CustomerType { get; set; } = CustomerType.Individual;

        // Bireyselde hesap sahibi, kurumsalda yetkili kişi
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Yalnızca kurumsal — bireyselde ünvan = Ad Soyad
        public string? CompanyTitle { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? CountryId { get; set; }

        // bireyselde kimlik no (TCKN · CPF · kimlik kartı), kurumsalda vergi no — biçim ülkeye göre
        public string TaxNumber { get; set; }

        // Yalnızca kurumsal — bireyselde tabloya "-" yazılır
        public string? TaxOffice { get; set; }

        // validator'da; tekrar alanı yazım hatasını yakalar
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
