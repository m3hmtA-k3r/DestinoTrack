using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
