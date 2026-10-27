namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    public class PersonelHomeViewModel
    {
        // Henüz açılmamış ekranların adları — çevrilmiş hâlleriyle gelir ( kurye yalnız kendi ekranını görür)
        public List<string> UpcomingScreens { get; set; } = new();
    }
}
