namespace DestinoTrack.DTO.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public Guid? CountryId { get; set; }   // Manager için zorunlu
        public Guid? BranchId { get; set; }    // Personel / Courier — şubeler  gelene kadar pasif

        // ikisi de boşsa şifre değişmez; doluysa kuralıyla yeni şifre atanır
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
