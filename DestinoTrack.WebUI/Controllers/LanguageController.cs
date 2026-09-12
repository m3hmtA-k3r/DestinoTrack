using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Controllers
{
    public class LanguageController : Controller
    {
        // Seçilen dili çereze yazar ve ziyaretçiyi geldiği sayfaya geri gönderir
        public IActionResult Change(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                });

            return LocalRedirect(returnUrl);
        }
    }
}
