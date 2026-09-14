using DestinoTrack.Business;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.WebUI.Areas.Admin.Models;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    public class CityController(ICityService _cityService, ICountryService _countryService, IStringLocalizer<SharedResource> _localizer) : Controller
    {

        public async Task<IActionResult> Index(Guid? countryId, string? q)
        {
            var model = new CityIndexViewModel
            {
                Cities = await _cityService.GetAllAsync(countryId, q),
                CountryFilters = await _cityService.GetCountryFiltersAsync(),
                SelectedCountryId = countryId,
                Search = q?.Trim()
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


        private async Task YukleCountriesAsync()
        {
            var countries = await _countryService.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
        }

    }
}
