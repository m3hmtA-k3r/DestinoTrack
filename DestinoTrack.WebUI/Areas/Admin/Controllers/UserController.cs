using DestinoTrack.Business;
using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Services.Branches;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Users;
using DestinoTrack.DTO.DTOs.UserDtos;
using DestinoTrack.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DestinoTrack.WebUI.Areas.Admin.Controllers
{
    public class UserController(IUserService _userService, ICountryService _countryService,
                            IBranchService _branchService, IStringLocalizer<SharedResource> _localizer) : AdminBaseController

    {    // [Authorize] sayesinde burada her zaman giriş yapmış bir kullanıcı var
        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Filtre oturumda tutulur: adres yalnızca sayfa numarası taşır
        private const string RoleKey = "user.role";
        private const string SearchKey = "user.q";

        public async Task<IActionResult> Index(int page = 1)
        {
            var role = HttpContext.Session.GetString(RoleKey);
            var search = HttpContext.Session.GetString(SearchKey);

            var model = new UserIndexViewModel
            {
                Users = await _userService.GetPagedAsync(role, search, page),
                RoleCounts = await _userService.GetRoleCountsAsync(),
                SelectedRole = role,
                Search = search,
                CurrentUserId = CurrentUserId
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await LoadFormListsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormListsAsync();
                return View(createUserDto);
            }

            var result = await _userService.CreateAsync(createUserDto);
            if (!result.Succeeded)
            {
                AddErrors(result, nameof(CreateUserDto.Password));
                await LoadFormListsAsync();
                return View(createUserDto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var value = await _userService.GetByIdAsync(id);
                await LoadFormListsAsync(value.Id);
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
        public async Task<IActionResult> Update(UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormListsAsync(updateUserDto.Id);
                return View(updateUserDto);
            }

            try
            {
                var result = await _userService.UpdateAsync(updateUserDto, CurrentUserId);
                if (!result.Succeeded)
                {
                    AddErrors(result, nameof(UpdateUserDto.NewPassword));
                    await LoadFormListsAsync(updateUserDto.Id);
                    return View(updateUserDto);
                }
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(Guid id, bool isActive)   // silme yok — pasifleştir / aktifleştir
        {
            try
            {
                var result = await _userService.SetActiveAsync(id, isActive, CurrentUserId);
                if (!result.Succeeded)
                {
                    TempData["Error"] = result.Errors.First().Description;
                }
            }
            catch (ValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Hata kodu → formdaki alan. Eşleşmeyen kod formun üstündeki banda (boş anahtar) yazılır
        private void AddErrors(IdentityResult result, string passwordField)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    continue;
                }

                var field = error.Code switch
                {
                    "DuplicateEmail" or "InvalidEmail" or "InvalidUserName" => nameof(CreateUserDto.Email),
                    "RoleInvalid" or "CannotChangeOwnRole" or "LastAdminProtected" => nameof(CreateUserDto.Role),
                    "CountryRequiredForManager" or "CountryNotFound" => nameof(CreateUserDto.CountryId),
                    _ when error.Code.StartsWith("Password") => passwordField,
                    _ => string.Empty
                };

                ModelState.AddModelError(field, error.Description);
            }
        }

        // Rol  ve arama kutusu buraya gönderir; kaydedip listeye döner 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(string? role, string? q)
        {
            // Listede olmayan bir rol gönderilirse filtre yok sayılır
            if (!string.IsNullOrEmpty(role) && RoleNames.Staff.Contains(role))
            {
                HttpContext.Session.SetString(RoleKey, role);
            }
            else
            {
                HttpContext.Session.Remove(RoleKey);
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

        // Rol ve ülke listeleri — form ilk açıldığında da, hatayla geri döndüğünde de doldurulur
        private async Task LoadFormListsAsync(Guid? editingUserId = null)
        {
            ViewBag.Roles = RoleNames.Staff
                .Select(r => new SelectListItem(_localizer["Role_" + r].Value, r))
                .ToList();

            var countries = await _countryService.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");

            // Personel / Kurye rolünde şube seçilebilir — "Ad · Şehir" biçiminde
            var branches = await _branchService.GetLookupAsync();
            ViewBag.Branches = branches
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = $"{b.Name} · {b.CityName}" })
                .ToList();

            //kendi kaydını düzenleyen Admin rol alanını kilitli görür
            ViewBag.IsSelf = editingUserId == CurrentUserId;
        }
    }
}
