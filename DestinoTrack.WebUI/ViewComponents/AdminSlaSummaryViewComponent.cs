using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminSlaSummaryViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sla = await _cargoService.GetSlaSummaryAsync();

            // Kısıtsız olmayan kapsama null; rapor sayfası Admin kilitli olduğu için buraya düşülemez
            if (sla is null)
            {
                return Content(string.Empty);
            }

            return View(sla);
        }
    }
}
