using DestinoTrack.Business;
using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.DTO.DTOs.BranchDtos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public abstract class BranchControllerBase(IBranchService _branchService, ICityService _cityService,
                                               ICountryService _countryService, IStringLocalizer<SharedResource> _localizer) : AdminBaseController
    {
        // Tesis ekranlarının ortak tabanı: liste · filtre · ekle · düzenle · sil.
        // Şube ekranı bütün türleri gösterir; transfer merkezi ekranı tek türe kilitlidir.
        // abstract: MVC bunu kendi başına bir controller saymaz, yalnızca türeyenler adres alır.

        // Bu ekranın kilitli olduğu tesis türü — null: bütün türler (şube ekranı)
        protected abstract BranchType? LockedType { get; }

        // Oturum anahtarlarının ön eki: iki ekranın filtreleri birbirine karışmasın diye (branch.q · tc.q)
        protected abstract string SessionPrefix { get; }

        private string TypeKey => $"{SessionPrefix}.type";
        private string SearchKey => $"{SessionPrefix}.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            // Kilitliyse tür oturumdan değil, ekranın kendisinden gelir
            var type = LockedType
                ?? (Enum.TryParse<BranchType>(HttpContext.Session.GetString(TypeKey), out var t) ? t : (BranchType?)null);
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new BranchIndexViewModel
            {
                Branches = await _branchService.GetPagedAsync(type, search, page),
                // Kilitli ekranda tür çipi gösterilmez
                TypeFilters = LockedType is null ? await _branchService.GetTypeFiltersAsync() : new(),
                SelectedType = type,
                Search = search
            };

            return BranchView("Index", model);
        }

        // Tür çipleri ve arama kutusu buraya gönderir; kaydedip listeye döner (adres temiz kalır)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(BranchType? branchType, string? q)
        {
            // Kilitli ekranda tür seçilemez; yalnız arama saklanır
            if (LockedType is null && branchType.HasValue && Enum.IsDefined(branchType.Value))
            {
                HttpContext.Session.SetString(TypeKey, branchType.Value.ToString());
            }
            else
            {
                HttpContext.Session.Remove(TypeKey);
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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            await YukleFormListeleriAsync();

            // Kilitli ekranda tür baştan dolu gelir (formda gizli alan olarak taşınır)
            return BranchView("Create", new CreateBranchDto { BranchType = LockedType ?? BranchType.Branch });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBranchDto createBranchDto)
        {
            // Form elle değiştirilse bile kilitli ekran kendi türünü yazar
            if (LockedType is not null)
            {
                createBranchDto.BranchType = LockedType.Value;
            }

            if (!ModelState.IsValid)
            {
                await YukleFormListeleriAsync();
                return BranchView("Create", createBranchDto);
            }

            try
            {
                await _branchService.CreateAsync(createBranchDto);
            }
            catch (DbUpdateException)
            {
                // Code unique (filtreli indeks) — aynı tesis kodu ikinci kez eklenirse buraya düşer
                ModelState.AddModelError(nameof(createBranchDto.Code), _localizer["BranchCodeTaken"].Value);
                await YukleFormListeleriAsync();
                return BranchView("Create", createBranchDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                await EnsureInScopeAsync(id);
                var branch = await _branchService.GetByIdAsync(id);
                await YukleFormListeleriAsync();
                return BranchView("Update", branch);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateBranchDto updateBranchDto)
        {
            if (LockedType is not null)
            {
                updateBranchDto.BranchType = LockedType.Value;
            }

            if (!ModelState.IsValid)
            {
                await YukleFormListeleriAsync();
                return BranchView("Update", updateBranchDto);
            }

            try
            {
                await EnsureInScopeAsync(updateBranchDto.Id);
                await _branchService.UpdateAsync(updateBranchDto);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(updateBranchDto.Code), _localizer["BranchCodeTaken"].Value);
                await YukleFormListeleriAsync();
                return BranchView("Update", updateBranchDto);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await EnsureInScopeAsync(id);
                await _branchService.DeleteAsync(id);
            }
            catch (ValidationException ex)
            {
                // Bağlı kargo · personel · ödeme · müdür varsa servis buraya düşürür 
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Kilitli ekranda başka türden bir kayda dokunulamaz
        // (ör. transfer merkezi ekranının adresine bir şubenin kimliği yazılırsa "bulunamadı" denir)
        private async Task EnsureInScopeAsync(Guid id)
        {
            if (LockedType is null)
            {
                return;
            }

            var branch = await _branchService.GetByIdAsync(id);   // yoksa zaten "Şube bulunamadı" dondurur
            if (branch.BranchType != LockedType)
            {
                throw new ValidationException(_localizer["BranchNotFound"].Value);
            }
        }

        // İki ekran da Views/Branch altındaki aynı görünümleri kullanır; görünüm kilitli türü ViewBag'den okur
        private ViewResult BranchView(string name, object? model)
        {
            ViewBag.LockedType = LockedType;
            return View($"~/Areas/Admin/Views/Branch/{name}.cshtml", model);
        }

        // Formlardaki ülke ve şehir listeleri: şehirler ülkeye göre süzülür (data-parent)
        private async Task YukleFormListeleriAsync()
        {
            ViewBag.Countries = (await _countryService.GetAllAsync())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            ViewBag.Cities = await _cityService.GetLookupAsync();
        }
    }
}
