using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Employees;
using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class EmployeeController(IEmployeeService _employeeService, IBranchService _branchService,
                                ICountryService _countryService) : AdminBaseController

    {
        // Filtre oturumda tutulur: adres yalnızca sayfa numarası taşır
        private const string JobTypeKey = "employee.jobType";
        private const string SearchKey = "employee.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var jobType = Enum.TryParse<EmployeeJobType>(HttpContext.Session.GetString(JobTypeKey), out var t) ? t : (EmployeeJobType?)null;
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new EmployeeIndexViewModel
            {
                Employees = await _employeeService.GetPagedAsync(jobType, search, page),
                JobTypeFilters = await _employeeService.GetJobTypeFiltersAsync(),
                SelectedJobType = jobType,
                Search = search
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(EmployeeJobType? jobType, string? q)
        {
            if (jobType.HasValue && Enum.IsDefined(jobType.Value))
            {
                HttpContext.Session.SetString(JobTypeKey, jobType.Value.ToString());
            }
            else
            {
                HttpContext.Session.Remove(JobTypeKey);
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
            return View(new CreateEmployeeDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleFormListeleriAsync();
                return View(createEmployeeDto);
            }

            var result = await _employeeService.CreateAsync(createEmployeeDto);
            if (!result.Succeeded)
            {
                AddErrors(result);
                await YukleFormListeleriAsync();
                return View(createEmployeeDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                await YukleFormListeleriAsync();
                return View(employee);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                await YukleFormListeleriAsync();
                return View(updateEmployeeDto);
            }

            try
            {
                await _employeeService.UpdateAsync(updateEmployeeDto);
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        //pasifleştir / aktifleştir — bağlı giriş hesabı da kilitlenir / açılır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(Guid id, bool isActive)
        {
            try
            {
                await _employeeService.SetActiveAsync(id, isActive);
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
                await _employeeService.DeleteAsync(id);
            }
            catch (ValidationException ex)
            {
                // Bağlı kargo / ödeme varsa servis buraya düşürür
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Identity hata kodu → formdaki alan (Kullanıcılar ekranındaki eşlemenin aynısı)
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                // Kullanıcı adı = e-posta: aynı hata DuplicateEmail olarak zaten yazılıyor
                if (error.Code == "DuplicateUserName")
                {
                    continue;
                }

                var field = error.Code switch
                {
                    "DuplicateEmail" or "InvalidEmail" or "InvalidUserName" => nameof(CreateEmployeeDto.Email),
                    _ when error.Code.StartsWith("Password") => nameof(CreateEmployeeDto.Password),
                    _ => string.Empty
                };

                ModelState.AddModelError(field, error.Description);
            }
        }

        // Formlardaki ülke ve tesis listeleri: tesisler ülkeye göre süzülür
        private async Task YukleFormListeleriAsync()
        {
            ViewBag.Countries = (await _countryService.GetAllAsync())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            ViewBag.Branches = await _branchService.GetLookupAsync();
        }
    }
}
