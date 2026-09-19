using DestinoTrack.Business;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class CityController(ICityService _cityService, ICountryService _countryService, IStringLocalizer<SharedResource> _localizer) : AdminBaseController
    {
        // Filtre oturumda tutulur: adres yalnızca sayfa numarası taşır
        private const string CountryKey = "city.countryId";
        private const string SearchKey = "city.q";
        public async Task<IActionResult> Index(int page = 1)
        {
            var countryId = Guid.TryParse(HttpContext.Session.GetString(CountryKey), out var id) ? id : (Guid?)null;
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new CityIndexViewModel
            {
                Cities = await _cityService.GetPagedAsync(countryId, search, page),
                CountryFilters = await _cityService.GetCountryFiltersAsync(),
                SelectedCountryId = countryId,
                Search = search
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await YukleCountriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCityDto createCityDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleCountriesAsync();
                return View(createCityDto);
            }

            try
            {
                await _cityService.CreateAsync(createCityDto);
            }
            catch (DbUpdateException)
            {
                // (CountryId, Name) unique — aynı ülkede aynı şehir ikinci kez eklenirse buraya düşer
                ModelState.AddModelError(nameof(createCityDto.Name), _localizer["CityNameTaken"].Value);
                await YukleCountriesAsync();
                return View(createCityDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var value = await _cityService.GetByIdAsync(id);
                await YukleCountriesAsync();
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
        public async Task<IActionResult> Update(UpdateCityDto updateCityDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleCountriesAsync();
                return View(updateCityDto);
            }

            try
            {
                await _cityService.UpdateAsync(updateCityDto);
            }
            catch (DbUpdateException)
            {
                // Başka bir şehrin adı verilirse (CountryId, Name) unique index engeller
                ModelState.AddModelError(nameof(updateCityDto.Name), _localizer["CityNameTaken"].Value);
                await YukleCountriesAsync();
                return View(updateCityDto);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _cityService.DeleteAsync(id);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));

        }

        // Çipler ve arama kutusu buraya gönderir; kaydedip listeye döner (adres temiz kalır)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(Guid? countryId, string? q)
        {
            if (countryId.HasValue)
            {
                HttpContext.Session.SetString(CountryKey, countryId.Value.ToString());
            }
            else
            {
                HttpContext.Session.Remove(CountryKey);
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

            // Filtre değişti → her zaman 1. sayfadan başla
            return RedirectToAction(nameof(Index));
        }

        private async Task YukleCountriesAsync()
        {
            var countries = await _countryService.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
        }

    }
}
