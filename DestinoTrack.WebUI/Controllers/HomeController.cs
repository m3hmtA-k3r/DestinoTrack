using DestinoTrack.Business;
using DestinoTrack.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Controllers
{
    public class HomeController(IStringLocalizer<SharedResource> _localizer) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Takip sorgusu: aramayı TrackController yapar, burası yalnız adrese çevirir 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Track(string code)
        {
            code = (code ?? string.Empty).Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(code))
            {
                TempData["TrackNotice"] = _localizer["TrackNotFound"].Value;

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index", "Track", new { code });
        }


        public IActionResult Privacy()
        {
            return View();
        }

        // Beklenmeyen hata UseExceptionHandler isteği buraya yeniden çalıştırır
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        // Gövdesi boş hata kodları UseStatusCodePagesWithReExecute isteği buraya yeniden çalıştırır
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HttpStatus(int id)
        {
            if (id == StatusCodes.Status404NotFound)
            {
                return View("NotFound");
            }

            return View("HttpStatus", id);
        }
    }
}
