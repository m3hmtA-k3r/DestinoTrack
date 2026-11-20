using DestinoTrack.Business.Services.Cargos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Manager.Controllers
{
    // Ülkenin sorunlu kargoları. Rol kilidi ve area adı ManagerBaseController'da,
    // o yüzden burada attribute yok — CargoController ile aynı duruş
    public class IssueController(ICargoService _cargoService) : ManagerBaseController
    {
        private const string IssueKey = "country.issue.kind";

        public async Task<IActionResult> Index(int page = 1)
        {
            var issue = IssueFromSession();

            var model = new CargoIssueListViewModel
            {
                Cargos = await _cargoService.GetIssuesAsync(issue, page),
                SelectedIssue = issue
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(CargoIssue? issue)
        {
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
