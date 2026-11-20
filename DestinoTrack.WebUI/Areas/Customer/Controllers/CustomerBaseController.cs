using DestinoTrack.Business.Consts;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Customer.Controllers
{
    // Müşteri panelindeki bütün controller'lar buradan türeyecek.
    [Area(AreaNames.Customer)]
    [Authorize(Roles = RoleNames.Customer)]
    public abstract class CustomerBaseController : Controller
    {
    }
}
