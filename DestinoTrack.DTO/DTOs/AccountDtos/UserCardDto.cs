namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    public class UserCardDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty; // Avatar: "TY"
        public string Role { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } // Yönetim Paneli linki yalnızca Admin'e
        public bool IsManager { get; set; } // Ülke Paneli linki yalnızca Manager'a
        public bool IsBranchStaff { get; set; } // Şube Paneli linki Personel ve Kurye'ye

    }
}
