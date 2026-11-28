using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.CustomerDtos
{
    public class UpdateCustomerDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        // İkisi de değiştirilebilir: şahıs şirketi kurumsallaşabilir, müşteri ülke değiştirebilir.
        // Değişince vergi numarası yeni kurala göre yeniden doğrulanır
        public CustomerType CustomerType { get; set; }
        public Guid CountryId { get; set; }

        public string TaxNumber { get; set; }
        public string TaxOffice { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // SALT OKUNUR — yalnız ekranda gösterilir. UpdateAsync bu ikisini YAZMAZ:
        // kod üretildiği gibi kalır, durum yalnız durum eylemleriyle değişir (D114 · D115)
        public string Code { get; set; }
        public AccountStatus Status { get; set; }
    }
}
