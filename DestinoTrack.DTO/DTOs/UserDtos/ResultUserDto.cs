namespace DestinoTrack.DTO.DTOs.UserDtos
{
    public class ResultUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Her kullanıcının tek rolü var 
        public string Role { get; set; }

        public string? CountryName { get; set; }   // Manager: atanmadıysa null
        public string? BranchName { get; set; }    // Personel / Courier: atanmadıysa null

        // pasifleştirilen kullanıcı giriş yapamaz
        public bool IsActive { get; set; }
    }
}
