using DestinoTrack.Business.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.ViewComponents
{
    // Admin layout'undaki kullanıcı kartı ve başlık avatarı: veriyi controller'dan değil servisten alır 
    public class AdminUserCardViewComponent(IAccountService _accountService) : ViewComponent
    {       
        public async Task<IViewComponentResult> InvokeAsync(string view = "Default")
        {
            var card = await _accountService.GetUserCardAsync(UserClaimsPrincipal);
            if (card == null)
            {
                // Çerez var ama kullanıcı silinmis kart çizilmez, sayfa yine açılır
                return Content(string.Empty);
            }

            return View(view, card);
        }
    }
}
