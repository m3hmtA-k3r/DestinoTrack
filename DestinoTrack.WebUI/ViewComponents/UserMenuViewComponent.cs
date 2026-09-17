using DestinoTrack.Business.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // Public başlıktaki hesap menüsü: girişsize Giriş Yap / Kayıt Ol, girişliye hesap menüsü  
    public class UserMenuViewComponent(IAccountService _accountService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var card = await _accountService.GetUserCardAsync(UserClaimsPrincipal);

            if (card == null)
            {
                return View("Anonymous");
            }

            return View(card);
        }
    }
}
