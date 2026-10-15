namespace DestinoTrack.Business.Options
{
    // appsettings "Cargo" bölümü. Değer yoksa buradaki varsayılanlar geçerli
    public class CargoSettings
    {
        // takip no {Önek}-{Yıl}-{6 rakam} → MYC-2026-849251
        public string TrackCodePrefix { get; set; } = "MYC";

        // bu kadar başarısız denemeden sonra iade süreci başlar
        public int MaxDeliveryAttempts { get; set; } = 3;


        //  Etiket üzerindeki barkod ve QR 

        // Barkod yüksekliği — genişliği içerik belirler
        public int BarcodeHeight { get; set; } = 50;

        // Barkodun iki yanındaki boşluk. Okuyucu barkodun nerede
        public int BarcodeMargin { get; set; } = 10;

        // QR kenar uzunluğu ve çevresindeki boşluk
        public int QrSize { get; set; } = 160;
        public int QrMargin { get; set; } = 1;

        // QR'ın taşıdığı adres; {0} takip numarasıyla değişir.
        public string PublicTrackingUrlFormat { get; set; } = "";
    }
}
