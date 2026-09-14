namespace DestinoTrack.DTO.DTOs.UserDtos
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        //validator'da; tekrar alanı yazım hatasını yakalar
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }

        public Guid? CountryId { get; set; }   // Manager için zorunlu
        public Guid? BranchId { get; set; }    // Personel / Courier — şubeler gelene kadar pasif
    }
}
