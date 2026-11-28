using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminDeliveryPerformanceViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var personel = await _cargoService.GetDeliveryPerformanceAsync();

            // Kısıtsız olmayan kapsama null; rapor sayfası Admin kilitli olduğu için buraya düşülemez
            if (personel is null)
            {
                return Content(string.Empty);
            }

            return View(personel);
        }
    }
}
