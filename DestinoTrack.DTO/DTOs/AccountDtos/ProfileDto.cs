using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    public class ProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new();

        public string? CountryName { get; set; }   // Manager
        public string? BranchName { get; set; }    // Personel · Courier

        // Yalnızca müşteri hesabında dolar
        public string? CustomerCode { get; set; }
        public string? CustomerTitle { get; set; }
        public CustomerType? CustomerType { get; set; }
        public AccountStatus? CustomerStatus { get; set; }
    }
}
