namespace DestinoTrack.WebUI.Areas.Manager.Models
{
    // Ülke panelinin giriş sayfası. Sayıları görünüm bileşenleri servisten kendisi alıyor —
    // bu model yalnız ekranın HANGİ durumu çizeceğine karar vermek için gerekli bilgiyi taşır
    public class ManagerHomeViewModel
    {
        public string? CountryName { get; set; }
        public int BranchCount { get; set; }
    }
}
