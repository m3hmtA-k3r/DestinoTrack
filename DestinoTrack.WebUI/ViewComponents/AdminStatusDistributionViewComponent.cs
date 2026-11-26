using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminStatusDistributionViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dagilim = await _cargoService.GetStatusDistributionAsync();

            // Kısıtsız olmayan kapsama null döner; pano Admin kilitli olduğu için buraya
            // düşülemez, erişilemeyecek bir görünüm kurmuyoruz
            if (dagilim is null)
            {
                return Content(string.Empty);
            }

            return View(dagilim);
        }
    }
}
