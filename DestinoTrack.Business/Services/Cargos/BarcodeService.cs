using DestinoTrack.Business.Options;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace DestinoTrack.Business.Services.Cargos
{
    public class BarcodeService(IOptions<CargoSettings> _cargoSettings,
                                IStringLocalizer<SharedResource> _localizer) : IBarcodeService
    {
        // Bir kargo etiketine sığan en uzun barkod; fazlası çizgileri inceltir, okuyucu zorlanır
        private const int MaxBarcodeLength = 48;

        // QR'a koyduğumuz en uzun içerik (takip adresi)
        private const int MaxQrLength = 300;


        public string Code128Svg(string value)
        {
            // Code128 yalnız ASCII taşır — "İ" gibi bir karakterde ZXing'in hatası yerine kendi mesajımız
            if (string.IsNullOrWhiteSpace(value) || value.Length > MaxBarcodeLength || value.Any(ch => ch > 127))
            {
                throw new ValidationException(_localizer["BarcodeValueInvalid"].Value);
            }

            var settings = _cargoSettings.Value;

            var writer = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = settings.BarcodeHeight,
                    Margin = settings.BarcodeMargin,
                    PureBarcode = true   // rakamları görsele gömmez; yazıyı etiketin kendisi basar
                }
            };

            return Inline(writer.Write(value).Content);
        }


        public string QrSvg(string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > MaxQrLength)
            {
                throw new ValidationException(_localizer["BarcodeValueInvalid"].Value);
            }

            var settings = _cargoSettings.Value;

            var writer = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = settings.QrSize,
                    Height = settings.QrSize,
                    Margin = settings.QrMargin,

                    // kodun ~%15'i zarar görse de okunur. 
                    ErrorCorrection = ErrorCorrectionLevel.M,
                    CharacterSet = "UTF-8"
                }
            };

            return Inline(writer.Write(content).Content);
        }


        // ZXing çıktının başına XML bildirimi, yorum ve DOCTYPE ekler.
        // Bu SVG bir HTML sayfasının içine gömülecek; orada yalnız <svg> öğesi geçerli
        private static string Inline(string svg)
        {
            var start = svg.IndexOf("<svg", StringComparison.Ordinal);
            return start < 0 ? svg : svg[start..];
        }
    }
}
