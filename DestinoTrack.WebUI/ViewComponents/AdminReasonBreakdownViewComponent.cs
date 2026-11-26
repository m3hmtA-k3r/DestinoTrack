using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class AdminReasonBreakdownViewComponent(ICargoService _cargoService) : ViewComponent
    {
        // kind: "delay" = yolda neden gecikti · "failure" = kapıda neden teslim edilemedi.
        // Tek bileşen iki kartı çiziyor; ikinci bir görünüm kopyası yazılmıyor
        public async Task<IViewComponentResult> InvokeAsync(string kind = "delay")
        {
            var dagilim = kind == "failure"
                ? await _cargoService.GetFailureReasonBreakdownAsync()
                : await _cargoService.GetDelayReasonBreakdownAsync();

            if (dagilim is null)
            {
                return Content(string.Empty);
            }

            return View(dagilim);
        }
    }
}
