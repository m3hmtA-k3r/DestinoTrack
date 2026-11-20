using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Customer.Controllers
{
    public class HomeController : CustomerBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
