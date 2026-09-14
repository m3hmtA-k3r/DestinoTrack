using DestinoTrack.Business;
using DestinoTrack.Business.Consts;
using DestinoTrack.DTO.DTOs.AccountDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.WebUI.Consts;
using DestinoTrack.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.WebUI.Controllers
{
    public class AccountController(SignInManager<AppUser> _signInManager,
                                   UserManager<AppUser> _userManager,
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

            // lockoutOnFailure: true → 5 hatalı denemede 15 dakika kilit 
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, _localizer["AccountLocked"].Value);
                return View(loginDto);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, _localizer["InvalidLogin"].Value);
                return View(loginDto);
            }

            // Yalnızca site içi adrese dönülür
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null && await _userManager.IsInRoleAsync(user, RoleNames.Admin))
            {
                return RedirectToAction("Index", "City", new { area = AreaNames.Admin });
            }

            // Manager · Personel · Courier · Customer: paneller yazılana kadar Hesabım
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Çerez var ama kullanıcı artık yok — oturumu kapatıp girişe gönder
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new ProfileViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Roles = roles.ToList(),
                // Lazy loading: ülke ve şube ihtiyaç anında yüklenir
                CountryName = user.Country?.Name,
                BranchName = user.Branch?.Name
            };

            return View(model);
        }
    }
}
