using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Personel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Personel.Controllers
{
    // Sorunlu kargolar — yalnız Personel: kuryenin işi teslim denemesi, sorunun takibi şubenin
    [Authorize(Roles = RoleNames.Personel)]
    public class IssueController(ICargoService _cargoService) : PersonelBaseController
    {
        private const string IssueKey = "staff.issue.kind";

        public async Task<IActionResult> Index(int page = 1)
        {
            var issue = IssueFromSession();

            var model = new CargoIssueIndexViewModel
            {
                // Kapsamı servis uyguluyor: personel yalnız kendi şubesine değen kargoları görür
                Cargos = await _cargoService.GetIssuesAsync(issue, page),
                SelectedIssue = issue
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(CargoIssue? issue)
        {
            // Tanımsız değer saklanmaz: adres çubuğundan "99" gelirse filtre bozulmasın
            if (issue.HasValue && Enum.IsDefined(issue.Value))
            {
                HttpContext.Session.SetString(IssueKey, issue.Value.ToString());
            }
            else
            {
                HttpContext.Session.Remove(IssueKey);
            }

            return RedirectToAction(nameof(Index));
        }


        // Oturumdaki metin geçerli bir küme değilse filtre yokmuş gibi davranır
        private CargoIssue? IssueFromSession()
        {
            var saved = HttpContext.Session.GetString(IssueKey);

            if (Enum.TryParse<CargoIssue>(saved, out var issue) && Enum.IsDefined(issue))
            {
                return issue;
            }

            return null;
        }
    }
}
