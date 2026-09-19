using DestinoTrack.Business;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.DTO.DTOs.CountryDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class CountryController(ICountryService _countryService, IStringLocalizer<SharedResource> _localizer) : AdminBaseController
    {
        public async Task<IActionResult> Index(int page = 1)
        {
            var values = await _countryService.GetPagedAsync(page);
            return View(values);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCountryDto createCountryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createCountryDto);
            }
            try
            {
                await _countryService.CreateAsync(createCountryDto);
            }
            catch (DbUpdateException)
            {
                // aynı kod ikinci kez girilirse buraya düşer
                ModelState.AddModelError(nameof(createCountryDto.IsoCode), _localizer["IsoCodeTaken"].Value);
                return View(createCountryDto);
            }


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var value = await _countryService.GetByIdAsync(id);
                return View(value);
            }
            catch (ValidationException ex)
            {
                // Kayıt yok (başka sekmede silinmiş ya da adres elle yazılmış) — servis "Ülke bulunamadı." der
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCountryDto updateCountryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateCountryDto);
            }

            try
            {
                await _countryService.UpdateAsync(updateCountryDto);
            }
            catch (DbUpdateException)
            {
                // Başka bir ülkenin ISO kodu verilirse unique index engeller
                ModelState.AddModelError(nameof(updateCountryDto.IsoCode), _localizer["IsoCodeTaken"].Value);
                return View(updateCountryDto);
            }
            catch (ValidationException ex)
            {
                // Form açıkken kayıt başka sekmede silinmiş olabilir
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
                await _countryService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                //bağlı şehir veya müşteri varken veritabanı silmeyi engeller
                TempData["Error"] = _localizer["CountryHasDependents"].Value;
            }
            catch (ValidationException ex)
            {
                // Aynı kayıt iki sekmeden silinmek istenirse ikincisi buraya düşer
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
