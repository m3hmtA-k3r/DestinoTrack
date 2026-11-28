using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Customers;
using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class CustomerController(ICustomerService _customerService,
                                    ICountryService _countryService) : AdminBaseController
    {
        // Süzgeçler oturumda: adres yalnızca sayfa numarası taşır (personel ekranındaki kural).
        // Anahtarlar "customer." önekli — başka panelin süzgeciyle karışmasın
        private const string StatusKey = "customer.status";
        private const string TypeKey = "customer.type";
        private const string SearchKey = "customer.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var status = Enum.TryParse<AccountStatus>(HttpContext.Session.GetString(StatusKey), out var s) ? s : (AccountStatus?)null;
            var type = Enum.TryParse<CustomerType>(HttpContext.Session.GetString(TypeKey), out var t) ? t : (CustomerType?)null;
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new CustomerIndexViewModel
            {
                Customers = await _customerService.GetPagedAsync(status, type, search, page),
                StatusFilters = await _customerService.GetStatusFiltersAsync(),
                SelectedStatus = status,
                SelectedType = type,
                Search = search
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(AccountStatus? status, CustomerType? customerType, string? q)
        {
            // Tanımsız enum değeri süzgeç sayılmaz: elle yazılmış adres "Tümü" gibi davransın
            SaveFilter(StatusKey, status.HasValue && Enum.IsDefined(status.Value) ? status.Value.ToString() : null);
            SaveFilter(TypeKey, customerType.HasValue && Enum.IsDefined(customerType.Value) ? customerType.Value.ToString() : null);
            SaveFilter(SearchKey, q?.Trim());

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            await YukleFormListeleriAsync();

            return View(new CreateCustomerDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerDto createCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleFormListeleriAsync();

                return View(createCustomerDto);
            }

            try
            {
                await _customerService.CreateAsync(createCustomerDto);
            }
            catch (ValidationException ex)
            {
                // Vergi numarası biçimi / tekilliği serviste denetleniyor — mesaj formun üstünde çıkar
                ModelState.AddModelError(string.Empty, ex.Message);
                await YukleFormListeleriAsync();

                return View(createCustomerDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                await YukleFormListeleriAsync();

                return View(customer);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCustomerDto updateCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleFormListeleriAsync();

                return View(updateCustomerDto);
            }

            try
            {
                await _customerService.UpdateAsync(updateCustomerDto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await YukleFormListeleriAsync();

                return View(updateCustomerDto);
            }

            return RedirectToAction(nameof(Index));
        }

        // Onayla · askıya al · yeniden aktifleştir · kapat — dördü de aynı eylem.
        // Hangi geçişin serbest olduğunu CustomerService'teki tablo söyler, burada kural yok
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(Guid id, AccountStatus status)
        {
            try
            {
                await _customerService.SetStatusAsync(id, status);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Boş değer süzgeci kaldırır: "Tümü" için ayrı bir anahtar tutmuyoruz
        private void SaveFilter(string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                HttpContext.Session.Remove(key);
            }
            else
            {
                HttpContext.Session.SetString(key, value);
            }
        }

        private async Task YukleFormListeleriAsync()
        {
            ViewBag.Countries = (await _countryService.GetAllAsync())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }
    }
}
