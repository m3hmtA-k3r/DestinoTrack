using DestinoTrack.Business.Consts;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    // Admin area'daki bütün controller'lar buradan türer: area adı ve rol kilidi tek yerde yaptık.

    // [Authorize]'lar VE ile birleşir: alt controller erişimi genişletemez. Diğer roller kendi area'sında kendi tabanını kullanır.
    [Area(AreaNames.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    public abstract class AdminBaseController : Controller
    {
    }
}
