namespace DestinoTrack.Business.Services.Cargos
{
    // Etiketteki QR'ın göstereceği herkese açık takip adresi 
    // Business katmanı host/port bilmez; adresi üreten sınıf WebUI'de
    // (ICurrentUserAccessor · HttpCurrentUserAccessor ile aynı kalıp)
    public interface IPublicTrackingUrl
    {
        // İstek yoksa (arka plan işi) null — o zaman QR takip numarasının kendisini taşır
        string? For(string trackCode);
    }
}
