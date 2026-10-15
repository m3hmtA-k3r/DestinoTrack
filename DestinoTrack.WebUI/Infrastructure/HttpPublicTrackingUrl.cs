using DestinoTrack.Business.Options;
using DestinoTrack.Business.Services.Cargos;
using Microsoft.Extensions.Options;

namespace DestinoTrack.WebUI.Infrastructure
{
    // QR'ın adresi : Ayar doluysa o, değilse çalışan uygulamanın kendi adresi
    public class HttpPublicTrackingUrl(IHttpContextAccessor _httpContextAccessor,
                                       LinkGenerator _linkGenerator,
                                       IOptions<CargoSettings> _cargoSettings) : IPublicTrackingUrl
    {
        public string? For(string trackCode)
        {
            // Ayar doldurulmuşsa gerçek alan adı iç adresten farklıdır; ayar kazanır
            var format = _cargoSettings.Value.PublicTrackingUrlFormat;
            if (!string.IsNullOrWhiteSpace(format))
            {
                return string.Format(format, trackCode);
            }

            var http = _httpContextAccessor.HttpContext;
            if (http == null)
            {
                // İstek yok (arka plan işi) — adres üretilemez, QR takip numarasını taşır
                return null;
            }

            // Adresi elle kurmuyoruz: yol değişirse QR kendiliğinden yeni adresi taşısın
            return _linkGenerator.GetUriByAction(http, action: "Index", controller: "Track",
                                                 values: new { code = trackCode });
        }
    }
}
