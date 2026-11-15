namespace DestinoTrack.WebUI.Areas.Personel.Models
{
    // Satır içi teslim/iade formlarının ihtiyacı yalnız bu ikisi.
    // Dağıtım listesi ve Yeni Hareket aynı partial'ı besliyor — işaretleme iki yerde yaşamasın
    public class DeliveryActionsViewModel
    {
        public Guid CargoId { get; set; }
        public Guid CourierId { get; set; }
    }
}
