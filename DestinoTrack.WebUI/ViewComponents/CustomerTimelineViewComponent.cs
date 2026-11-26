using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // VC12 · müşteri panosunun hareket çizelgesi + tahmini teslim.
    // VC11 ile aynı desen: veriyi servisten DTO olarak alıyor, panel controller'ı habersiz
    public class CustomerTimelineViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var timeline = await _cargoService.GetCustomerTimelineAsync();
            if (timeline is null)
            {
                return Content(string.Empty);
            }

            return View(timeline);
        }
    }
}
