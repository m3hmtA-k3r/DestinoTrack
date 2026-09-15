using DestinoTrack.Business;
using DestinoTrack.Business.Services.Accounts;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.DTO.DTOs.AccountDtos;
using DestinoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Controllers
{
    public class AccountController(IAccountService _accountService,
                                   ICountryService _countryService,
                                   IStringLocalizer<SharedResource> _localizer) : Controller
    {
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile)); // Zaten giriş yapmış kullanıcı formu tekrar görmez
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await _accountService.LoginAsync(loginDto);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(loginDto);
            }

            // Yalnızca site içi adrese dönülür
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            if (result.IsAdmin)
            {
                return RedirectToAction("Index", "City", new { area = AreaNames.Admin });
            }

            // Manager · Personel · Courier · Customer: paneller yazılana kadar Hesabım
            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile));
            }

            await LoadCountriesAsync();
            return View(new RegisterDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Profile));
            }

            if (!ModelState.IsValid)
            {
                await LoadCountriesAsync();
                return View(registerDto);
            }

            var result = await _accountService.RegisterCustomerAsync(registerDto);
            if (!result.Succeeded)
            {
                AddErrors(result);
                await LoadCountriesAsync();
                return View(registerDto);
            }

            // servis kullanıcıyı giriş de yaptırdı
            TempData["Success"] = _localizer["RegistrationCompleted"].Value;
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var profile = await _accountService.GetProfileAsync(User);
            if (profile == null)
            {
                // Çerez var ama kullanıcı artık yok — oturumu kapatıp girişe gönder
                await _accountService.LogoutAsync();
                return RedirectToAction(nameof(Login));
            }

            return View(profile);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    continue;
                }

                var field = error.Code switch
                {
                    "DuplicateEmail" or "InvalidEmail" or "InvalidUserName" => nameof(RegisterDto.Email),
                    "CountryNotFound" => nameof(RegisterDto.CountryId),
                    "TaxNumberTaken" => nameof(RegisterDto.TaxNumber),
                    _ when error.Code.StartsWith("TaxIdFormat_") => nameof(RegisterDto.TaxNumber),
                    _ when error.Code.StartsWith("Password") => nameof(RegisterDto.Password),
                    _ => string.Empty
                };

                ModelState.AddModelError(field, error.Description);
            }
        }

        // Kayıt formundaki ülke listesi — form ilk açıldığında da, hatayla geri döndüğünde de doldurulur
        private async Task LoadCountriesAsync()
        {
            var countries = await _countryService.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
        }
    }
}
