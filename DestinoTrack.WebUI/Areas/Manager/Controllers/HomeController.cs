using DestinoTrack.Business.Services.Countries;
using DestinoTrack.WebUI.Areas.Manager.Models;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Manager.Controllers
{
    public class HomeController(ICountryService _countryService) : ManagerBaseController
    {
        public async Task<IActionResult> Index()
        {
            var header = await _countryService.GetMyPanelHeaderAsync();

            var model = new ManagerHomeViewModel
            {
                // Ülkesi atanmamış ya da ülkesi silinmiş Manager: ad boş kalır, ekran bunu söyler
                CountryName = header?.Name,
                BranchCount = header?.BranchCount ?? 0
            };

            return View(model);
        }
    }
}
