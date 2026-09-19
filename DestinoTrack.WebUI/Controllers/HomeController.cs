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

        // Takip sorgusu. Cargo servisi yazıldığında burada gerçek arama yapılacak
        [HttpPost]
        public IActionResult Track(string code)
        {
            TempData["TrackCode"] = code;
            TempData["TrackNotice"] = _localizer["TrackNotice"].Value;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Beklenmeyen hata (500) · UseExceptionHandler isteği buraya yeniden çalıştırır
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        // Gövdesi boş hata kodları (404 · 400 · 405 …) · UseStatusCodePagesWithReExecute isteği buraya yeniden çalıştırır
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
