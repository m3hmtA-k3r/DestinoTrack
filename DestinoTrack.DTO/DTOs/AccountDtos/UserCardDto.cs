namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    // Layout'taki kullanıcı kartı ve üst menü — her sayfada çalışır, yalnızca gösterilecek kadar veri
    public class UserCardDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty; // Avatar: "TY"
        public string Role { get; set; } = string.Empty;       
        public bool IsAdmin { get; set; } // Yönetim Paneli linki yalnızca Admin'e
    }
}
