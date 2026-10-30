using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // VC— şubede en son yazılan hareketler. Kaç satır görüneceği çağıran ekranın kararı
    public class BranchRecentMovementsViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int count = 8)
        {
            var movements = await _cargoService.GetRecentMovementsAsync(count);

            // Şubesi atanmamış kullanıcı için servis boş liste döner — görünüm "kayıt yok" der
            return View(movements);
        }
    }
}
