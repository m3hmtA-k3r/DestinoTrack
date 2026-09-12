using DestinoTrack.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DestinoTrack.WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Takip sorgusu. Cargo servisi yazıldığında burada gerçek arama yapılacak
        // (CargoRepository.GetByTrackCodeAsync zaten hazır).
        [HttpPost]
        public IActionResult Track(string code)
        {
            TempData["TrackCode"] = code;
            TempData["TrackNotice"] = "Kargo modülü henüz tamamlanmadı. Takip sorgusu Cargo servisi eklendiğinde çalışacak.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
