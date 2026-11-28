using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminDelayedBranchesViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var subeler = await _cargoService.GetDelayedBranchesAsync();

            // Kısıtsız olmayan kapsama null; rapor sayfası Admin kilitli olduğu için buraya düşülemez
            if (subeler is null)
            {
                return Content(string.Empty);
            }

            return View(subeler);
        }
    }
}
