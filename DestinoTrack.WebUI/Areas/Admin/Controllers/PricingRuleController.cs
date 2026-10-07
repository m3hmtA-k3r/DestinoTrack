using DestinoTrack.Business;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Pricing;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class PricingRuleController(IPricingRuleService _pricingRuleService, ICountryService _countryService,
                                       IStringLocalizer<SharedResource> _localizer) : AdminBaseController
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

        // Sekiz çarpan satırı tek formda gelir, tek seferde kaydedilir 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRates(List<CargoTypeRateDto> rates)
        {
            if (!ModelState.IsValid)
            {
                // Satır içi düzenlemede ayrı form sayfası yok: hata bandıyla listeye dönülür
                TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
                return RedirectToAction(nameof(Index));
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
