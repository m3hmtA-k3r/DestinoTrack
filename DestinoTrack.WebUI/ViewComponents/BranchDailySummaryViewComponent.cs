using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // VC — şube panelinin günlük özeti. Veriyi controller'dan değil servisten alıyor:
    // sayıların hangi kapsamda üretileceğine servis karar veriyor (BranchSummary'deki kalıp)
    public class BranchDailySummaryViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var summary = await _cargoService.GetDailySummaryAsync();

            if (summary is null)
            {
                return View("NoBranch");
            }

            return View(summary);
        }
    }
}
