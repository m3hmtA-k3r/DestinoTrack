using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminSummaryViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var summary = await _cargoService.GetAdminSummaryAsync();

            // Kısıtsız olmayan kapsama null döner. Pano Admin kilitli olduğu için buraya
            // düşülemez; erişilemeyecek bir NoScope görünümü kurmuyoruz
            if (summary is null)
            {
                return Content(string.Empty);
            }

            return View(summary);
        }
    }
}
