using DestinoTrack.Business.Services.Countries;
using DestinoTrack.DTO.DTOs.CountryDtos;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    public class CountryController(ICountryService _countryService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var values = await _countryService.GetAllAsync();
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
            if(!ModelState.IsValid)
            {
                return View(createCountryDto);
            }
            try
            {
                await _countryService.CreateAsync(createCountryDto);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(createCountryDto.IsoCode), "Bu Iso kodu başka bir ülkeye kayıtlı");
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
                ModelState.AddModelError(nameof(updateCountryDto.IsoCode), "Bu ISO kodu başka bir ülkede kayıtlı.");
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
                TempData["Error"] = "Bu ülkeye bağlı şehir veya müşteri kaydı var. Önce onları silin.";
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
