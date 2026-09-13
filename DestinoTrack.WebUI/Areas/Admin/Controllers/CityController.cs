using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    public class CityController(ICityService _cityService, ICountryService _countryService) : Controller
    {
        
        public async Task<IActionResult> Index()
        {
            var values = await _cityService.GetAllAsync();
            return View(values);
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

            await _cityService.CreateAsync(createCityDto);
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
