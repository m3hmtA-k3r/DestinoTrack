using DestinoTrack.Business.Consts;
using DestinoTrack.WebUI.Areas.Personel.Models;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    public class HomeController : PersonelBaseController
    {
        public IActionResult Index()
        {
            // Kurye yalnız dağıtım ekranını görür — menü gibi ana sayfa da daralıyor.
            // Sayıları görünüm bileşenleri servisten kendisi alıyor, controller taşımıyor
            var model = new PersonelHomeViewModel
            {
                OnlyCourier = User.IsInRole(RoleNames.Courier) && !User.IsInRole(RoleNames.Personel)
            };

            return View(model);
        }
    }
}
