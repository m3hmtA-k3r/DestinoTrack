using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    //Manager ve Brancher panelinde günlük özet bilgilerini göstermek için kullanılan ViewComponent
    public class DailySummaryViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var summary = await _cargoService.GetDailySummaryAsync();

            if (summary is null)
            {
                return View("NoScope");
            }

            return View(summary);
        }
    }
}
