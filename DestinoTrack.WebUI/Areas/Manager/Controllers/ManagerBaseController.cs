using DestinoTrack.Business.Consts;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Manager.Controllers
{
    // Ülke panelindeki bütün controller'lar buradan türeyecek.
    [Area(AreaNames.Manager)]
    [Authorize(Roles = RoleNames.Manager)]
    public abstract class ManagerBaseController : Controller
    {
    }
}
