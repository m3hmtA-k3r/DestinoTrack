using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Employees;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    // OKUNUR: Create · Update · Delete bilerek tanımlanmadı.
    // Kurye kaydı Personel ekranından, kargo ataması şube panelinden yönetilir
    public class CourierController(IEmployeeService _employeeService, IBranchService _branchService) : AdminBaseController
    {
        private const string BranchKey = "courier.branchId";
        private const string TypeKey = "courier.type";
        private const string ActiveKey = "courier.active";
        private const string SearchKey = "courier.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var branchId = Guid.TryParse(HttpContext.Session.GetString(BranchKey), out var bid) ? bid : (Guid?)null;
            var vehicleType = Enum.TryParse<VehicleType>(HttpContext.Session.GetString(TypeKey), out var vt) ? vt : (VehicleType?)null;
            var isActive = bool.TryParse(HttpContext.Session.GetString(ActiveKey), out var act) ? act : (bool?)null;
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new CourierIndexViewModel
            {
                Couriers = await _employeeService.GetCourierOverviewAsync(branchId, vehicleType, isActive, search, page),
                Branches = await _branchService.GetLookupAsync(),
                SelectedBranchId = branchId,
                SelectedVehicleType = vehicleType,
                SelectedIsActive = isActive,
                Search = search
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(Guid? branchId, VehicleType? vehicleType, bool? isActive, string? q)
        {
            OturumaYaz(BranchKey, branchId?.ToString());
            OturumaYaz(TypeKey, vehicleType?.ToString());
            OturumaYaz(ActiveKey, isActive?.ToString());
            OturumaYaz(SearchKey, q?.Trim());

            return RedirectToAction(nameof(Index));
        }

        private void OturumaYaz(string anahtar, string? deger)
        {
            if (string.IsNullOrEmpty(deger))
            {
                HttpContext.Session.Remove(anahtar);
            }
            else
            {
                HttpContext.Session.SetString(anahtar, deger);
            }
        }
    }
}
