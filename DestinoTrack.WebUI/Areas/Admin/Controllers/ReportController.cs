using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class ReportController : AdminBaseController
    {
        public IActionResult Index()
        {
            // Pano ile aynı kalıp: sayıları görünüm bileşenleri servisten kendisi alıyor.
            // Area ve rol kilidi tabandan geliyor, burada [Area]/[Authorize] tekrarlanmıyor
            return View();
        }
    }
}
