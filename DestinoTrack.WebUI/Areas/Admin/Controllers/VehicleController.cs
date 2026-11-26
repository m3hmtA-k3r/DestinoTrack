using DestinoTrack.Business;
using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Vehicles;
using DestinoTrack.DTO.DTOs.VehicleDtos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class VehicleController(IVehicleService _vehicleService, IBranchService _branchService,
                                   IStringLocalizer<SharedResource> _localizer) : AdminBaseController
    {
        // Süzgeçler oturumda tutulur: adres yalnızca sayfa numarası taşır
        private const string BranchKey = "vehicle.branchId";
        private const string TypeKey = "vehicle.type";
        private const string ActiveKey = "vehicle.active";
        private const string SearchKey = "vehicle.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var branchId = Guid.TryParse(HttpContext.Session.GetString(BranchKey), out var bid) ? bid : (Guid?)null;
            var vehicleType = Enum.TryParse<VehicleType>(HttpContext.Session.GetString(TypeKey), out var vt) ? vt : (VehicleType?)null;
            var isActive = bool.TryParse(HttpContext.Session.GetString(ActiveKey), out var act) ? act : (bool?)null;
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new VehicleIndexViewModel
            {
                Vehicles = await _vehicleService.GetPagedAsync(branchId, vehicleType, isActive, search, page),
                Branches = await _branchService.GetLookupAsync(),
                SelectedBranchId = branchId,
                SelectedVehicleType = vehicleType,
                SelectedIsActive = isActive,
                Search = search
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await YukleBranchesAsync();

            // Yeni araç aktif açılır: pasif araç eklemek istisnadır
            return View(new CreateVehicleDto { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVehicleDto createVehicleDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleBranchesAsync();
                return View(createVehicleDto);
            }

            try
            {
                await _vehicleService.CreateAsync(createVehicleDto);
            }
            catch (ValidationException ex)
            {
                // Servisin plaka denetimi: aynı plaka başka bir araçta kayıtlı
                ModelState.AddModelError(nameof(createVehicleDto.Plate), ex.Message);
                await YukleBranchesAsync();
                return View(createVehicleDto);
            }
            catch (DbUpdateException)
            {
                // Son savunma: iki istek aynı anda gelirse servisin denetimi arada kalır, filtreli tekil indeks yakalar
                ModelState.AddModelError(nameof(createVehicleDto.Plate), _localizer["VehiclePlateInUse"].Value);
                await YukleBranchesAsync();
                return View(createVehicleDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var value = await _vehicleService.GetByIdAsync(id);
                await YukleBranchesAsync();
                return View(value);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateVehicleDto updateVehicleDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleBranchesAsync();
                return View(updateVehicleDto);
            }

            try
            {
                await _vehicleService.UpdateAsync(updateVehicleDto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(nameof(updateVehicleDto.Plate), ex.Message);
                await YukleBranchesAsync();
                return View(updateVehicleDto);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(updateVehicleDto.Plate), _localizer["VehiclePlateInUse"].Value);
                await YukleBranchesAsync();
                return View(updateVehicleDto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _vehicleService.DeleteAsync(id);
            }
            catch (ValidationException ex)
            {
                // Araca bağlı kurye varsa silinmez  
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Süzgeç kutuları buraya gönderir; oturuma yazıp listeye döner (adres temiz kalır)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(Guid? branchId, VehicleType? vehicleType, bool? isActive, string? q)
        {
            OturumaYaz(BranchKey, branchId?.ToString());
            OturumaYaz(TypeKey, vehicleType?.ToString());
            OturumaYaz(ActiveKey, isActive?.ToString());
            OturumaYaz(SearchKey, q?.Trim());

            // Süzgeç değişti → her zaman 1. sayfadan başla
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

        private async Task YukleBranchesAsync()
        {
            var branches = await _branchService.GetLookupAsync();
            ViewBag.Branches = new SelectList(branches, "Id", "Name");
        }
    }
}
