using DestinoTrack.Business.Consts;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    // Şube panelindeki bütün controller'lar buradan türeyecek
     [Area(AreaNames.Personel)]
    [Authorize(Roles = RoleNames.BranchPanel)]
    public abstract class PersonelBaseController : Controller
    {
    }
}
