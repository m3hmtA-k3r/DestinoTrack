using DestinoTrack.Business;
using DestinoTrack.Business.Consts;
using DestinoTrack.WebUI.Areas.Personel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    public class HomeController(IStringLocalizer<SharedResource> _localizer) : PersonelBaseController
    {
        public IActionResult Index()
        {
            // kurye yalnız dağıtım ekranını görür — menü gibi sayfanın kendisi de daralır
            var sadeceKurye = User.IsInRole(RoleNames.Courier) && !User.IsInRole(RoleNames.Personel);

            var model = new PersonelHomeViewModel();

            if (sadeceKurye)
            {
                model.UpcomingScreens.Add(_localizer["Menu_MyDeliveryList"].Value);
            }
            else
            {
                model.UpcomingScreens.Add(_localizer["Menu_BranchCargos"].Value);
                model.UpcomingScreens.Add(_localizer["Menu_NewMovement"].Value);
                model.UpcomingScreens.Add(_localizer["Menu_DeliveryList"].Value);
                model.UpcomingScreens.Add(_localizer["Menu_IssueCargos"].Value);
            }

            return View(model);
        }
    }
}
