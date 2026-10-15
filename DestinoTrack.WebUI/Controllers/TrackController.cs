using DestinoTrack.Business;
using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Controllers
{
    [EnableRateLimiting("track")]
    public class TrackController(ICargoService _cargoService,
                                 IStringLocalizer<SharedResource> _localizer) : Controller
    {
        // Varsayılan {controller}/{action}/{id} yolu takip numarasını action adı sanardı
        [HttpGet("/Track/{code?}")]
        public async Task<IActionResult> Index(string? code)
        {
            var tracking = await _cargoService.GetPublicTrackingAsync(code ?? string.Empty);

            if (tracking == null)
            {
                // Yanlış ya da bulunamayan numara: ana sayfadaki forma uyarıyla dön
                TempData["TrackCode"] = code;
                TempData["TrackNotice"] = _localizer["TrackNotFound"].Value;

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(tracking);
        }
    }
}
