using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminWeeklyTrafficViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var trafik = await _cargoService.GetWeeklyTrafficAsync();

            // Kısıtsız olmayan kapsama null; pano Admin kilitli olduğu için buraya düşülemez
            if (trafik is null)
            {
                return Content(string.Empty);
            }

            return View(trafik);
        }
    }
}
