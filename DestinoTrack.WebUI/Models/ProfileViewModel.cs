namespace DestinoTrack.WebUI.Models
{
    // Hesabım sayfası — entity değil, yalnızca ekranın ihtiyacı olan bilgiler
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new();

        public string? CountryName { get; set; }   // Manager
        public string? BranchName { get; set; }    // Personel · Courier
    }
}
