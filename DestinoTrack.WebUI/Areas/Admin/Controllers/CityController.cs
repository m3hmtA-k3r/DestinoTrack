using DestinoTrack.DataAccess.Repositories.Cities;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    public class CityController(ICityRepository _cityService) : Controller
    {
        
        public async Task<IActionResult> Index()
        {
            var values = await _cityService.GetAllAsync();
            return View(values);
        }



        
    }
}
