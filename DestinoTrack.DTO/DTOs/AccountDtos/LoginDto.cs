namespace DestinoTrack.DTO.DTOs.AccountDtos
{
    // Giriş formu. Kullanıcı adı olarak e-posta kullanılır — seed'deki Admin'de ve müşteri kaydında UserName = Email.
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        // İşaretlenirse çerez tarayıcı kapansa da korunur
        public bool RememberMe { get; set; }
    }
}
