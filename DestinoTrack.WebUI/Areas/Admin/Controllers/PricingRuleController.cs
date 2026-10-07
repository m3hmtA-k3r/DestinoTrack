using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Pricing;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.WebUI.Areas.Admin.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;


namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class PricingRuleController(IPricingRuleService _pricingRuleService, ICountryService _countryService,
                                   IValidator<CargoTypeRateDto> _rateValidator) : AdminBaseController
    {
        public async Task<IActionResult> Index()
        {
            return View(new PricingRuleIndexViewModel
            {
                Tariffs = await _pricingRuleService.GetTariffsAsync(),
                Rates = await _pricingRuleService.GetRatesAsync()
            });
        }

        public async Task<IActionResult> Create()
        {
            await YukleUlkeListesiAsync();
            return View(new CreateCargoPriceDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCargoPriceDto createCargoPriceDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleUlkeListesiAsync();
                return View(createCargoPriceDto);
            }

            try
            {
                await _pricingRuleService.CreateTariffAsync(createCargoPriceDto);
            }
            catch (ValidationException ex)
            {
                // Aynı ülke + kademe ikinci kez: hata formda, girilen değerler kaybolmasın
                ModelState.AddModelError(string.Empty, ex.Message);
                await YukleUlkeListesiAsync();
                return View(createCargoPriceDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var tariff = await _pricingRuleService.GetTariffByIdAsync(id);
                await YukleUlkeListesiAsync();
                return View(tariff);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCargoPriceDto updateCargoPriceDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleUlkeListesiAsync();
                return View(updateCargoPriceDto);
            }

            try
            {
                await _pricingRuleService.UpdateTariffAsync(updateCargoPriceDto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await YukleUlkeListesiAsync();
                return View(updateCargoPriceDto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _pricingRuleService.DeleteTariffAsync(id);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Sekiz çarpan satırı tek formda gelir, tek seferde kaydedilir (D39)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRates(List<CargoTypeRateDto> rates)
        {
            // Bağlama hataları (sayı yerine harf girilmesi gibi)
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
                return RedirectToAction(nameof(Index));
            }

            // FluentValidation'ın otomatik doğrulaması liste parametresinin satırlarına işlemiyor:
            // kuralları burada elle çalıştırıyoruz, yoksa 50 gibi bir çarpan sessizce kaydedilir
            foreach (var rate in rates)
            {
                var result = await _rateValidator.ValidateAsync(rate);
                if (!result.IsValid)
                {
                    TempData["Error"] = result.Errors[0].ErrorMessage;
                    return RedirectToAction(nameof(Index));
                }
            }

            await _pricingRuleService.UpdateRatesAsync(rates);
            return RedirectToAction(nameof(Index));
        }


        private async Task YukleUlkeListesiAsync()
        {
            ViewBag.Countries = (await _countryService.GetAllAsync())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Name} · {c.CurrencyCode}" })
                .ToList();
        }
    }
}
