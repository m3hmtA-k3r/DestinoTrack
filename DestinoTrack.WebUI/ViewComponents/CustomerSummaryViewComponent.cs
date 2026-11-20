using DestinoTrack.Business.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    public class CustomerSummaryViewComponent(ICargoService _cargoService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var summary = await _cargoService.GetCustomerSummaryAsync();

            if (summary is null)
            {
                return Content(string.Empty);
            }

            // "Hiç kargosu yok" durumu AYRI ve gerçekten olabilir (yeni müşteri) —
            // onu görünüm kendi içinde karşılıyor
            return View(summary);
        }
    }
}
