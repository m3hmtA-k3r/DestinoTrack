using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminTopBranchesViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var subeler = await _cargoService.GetTopBranchesAsync();

            // Kısıtsız olmayan kapsama null; pano Admin kilitli olduğu için buraya düşülemez
            if (subeler is null)
            {
                return Content(string.Empty);
            }

            return View(subeler);
        }
    }
}
