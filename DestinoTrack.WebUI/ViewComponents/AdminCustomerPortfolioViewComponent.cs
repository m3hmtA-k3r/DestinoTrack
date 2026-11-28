using DestinoTrack.Business.Services.Customers;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // VC16 · müşteri ekranının dört KPI kartı. Verisini kendi alır, ViewModel'e yük olmaz.
    // Adı "CustomerSummary" DEĞİL: o ad müşterinin kendi panelindeki özete ait
    public class AdminCustomerPortfolioViewComponent(ICustomerService _customerService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _customerService.GetPortfolioAsync());
        }
    }
}
