using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class RecentMovementsViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int count = 8)
        {
            var movements = await _cargoService.GetRecentMovementsAsync(count);

            // Kapsamsız kullanıcı için servis boş liste döner — görünüm "kayıt yok" der
            return View(movements);
        }
    }
}
