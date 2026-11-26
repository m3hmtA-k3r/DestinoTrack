using DestinoTrack.Business.Services.AuditLogs;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    // Salt okunur ekran: Create/Update/Delete BİLEREK yok. Denetim kaydı panelden değiştirilemez
    public class AuditController(IAuditLogService _auditLogService) : AdminBaseController
    {
        private const string ActionKey = "audit.action";
        private const string EntityKey = "audit.entity";
        private const string SearchKey = "audit.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var secilenIslem = Enum.TryParse<AuditAction>(HttpContext.Session.GetString(ActionKey), out var islem)
                               ? islem
                               : (AuditAction?)null;

            var entityName = HttpContext.Session.GetString(EntityKey);
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new AuditLogIndexViewModel
            {
                Logs = await _auditLogService.GetPagedAsync(secilenIslem, entityName, search, page),
                EntityFilters = await _auditLogService.GetEntityFiltersAsync(),
                SelectedAction = secilenIslem,
                SelectedEntityName = entityName,
                Search = search
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(AuditAction? auditAction, string? entityName, string? q)
        {
            if (auditAction.HasValue)
            {
                HttpContext.Session.SetString(ActionKey, auditAction.Value.ToString());
            }
            else
            {
                HttpContext.Session.Remove(ActionKey);
            }

            if (string.IsNullOrWhiteSpace(entityName))
            {
                HttpContext.Session.Remove(EntityKey);
            }
            else
            {
                HttpContext.Session.SetString(EntityKey, entityName.Trim());
            }

            var search = q?.Trim();
            if (string.IsNullOrEmpty(search))
            {
                HttpContext.Session.Remove(SearchKey);
            }
            else
            {
                HttpContext.Session.SetString(SearchKey, search);
            }

            // Süzgeç değişti → her zaman 1. sayfadan başla
            return RedirectToAction(nameof(Index));
        }
    }
}
