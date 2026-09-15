namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    // Girişin sonucu controller yalnızca buna bakıp yönlendirir
    public class LoginResultDto
    {
        public bool Succeeded { get; set; }

        // Genel hata 15 dakikalık kilit pasif hesabın hangisi olduğuna servis karar verir
        public string? ErrorMessage { get; set; }

        // Admin → Admin paneli, diğer roller → Hesabım 
        public bool IsAdmin { get; set; }
    }
}
