namespace DestinoTrack.Business.Options
{
    // appsettings "Cargo" bölümü. Değer yoksa buradaki varsayılanlar geçerli
    public class CargoSettings
    {
        // takip no {Önek}-{Yıl}-{6 rakam} → MYC-2026-849251
        public string TrackCodePrefix { get; set; } = "MYC";

        // bu kadar başarısız denemeden sonra iade süreci başlar
        public int MaxDeliveryAttempts { get; set; } = 3;
    }
}
