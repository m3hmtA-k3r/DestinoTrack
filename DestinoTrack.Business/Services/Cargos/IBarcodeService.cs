namespace DestinoTrack.Business.Services.Cargos
{
    // Barkod ve QR üretir
    public interface IBarcodeService
    {
        string Code128Svg(string value);

        // Telefonla okunan QR. İçerik takip numarası ya da takip adresi olabilir
        string QrSvg(string content);
    }
}
