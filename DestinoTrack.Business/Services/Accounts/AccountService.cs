using System.Security.Claims;
using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Services.Users;
using DestinoTrack.Business.Validators.Common;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DataAccess.Repositories.Customers;
using DestinoTrack.DTO.DTOs.AccountDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Services.Accounts
{
    public class AccountService(SignInManager<AppUser> _signInManager,
                                UserManager<AppUser> _userManager,
                                IUserService _userService,
                                ICustomerRepository _customerRepository,
                                ICountryRepository _countryRepository,
                                IStringLocalizer<SharedResource> _localizer) : IAccountService
    {
        public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return new LoginResultDto { ErrorMessage = await _userService.GetLockedOutMessageAsync(loginDto.Email, loginDto.Password) };
            }

            if (!result.Succeeded)
            {
                return new LoginResultDto { ErrorMessage = _localizer["InvalidLogin"].Value };
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            IList<string> roles = user is null ? [] : await _userManager.GetRolesAsync(user);

            return new LoginResultDto
            {
                Succeeded = true,
                IsAdmin = roles.Contains(RoleNames.Admin),

                IsManager = roles.Contains(RoleNames.Manager),

                IsBranchStaff = RoleNames.BranchPanelRoles.Any(roles.Contains)
            };


        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterCustomerAsync(RegisterDto registerDto)
        {
            var country = registerDto.CountryId == null ? null : await _countryRepository.GetByIdAsync(registerDto.CountryId.Value);
            if (country == null)
            {
                return Fail("CountryNotFound");
            }

            var taxNumber = TaxIdentityRules.Normalize(registerDto.TaxNumber);
            var formatErrorKey = TaxIdentityRules.GetFormatErrorKey(country.IsoCode, registerDto.CustomerType, taxNumber);
            if (formatErrorKey != null)
            {
                return Fail(formatErrorKey);
            }

            if (await _customerRepository.TaxNumberExistsAsync(country.Id, taxNumber))
            {
                return Fail("TaxNumberTaken");
            }

            var email = registerDto.Email.Trim();
            var phone = registerDto.PhoneNumber.Trim();
            var user = new AppUser
            {
                UserName = email,         
                Email = email,
                EmailConfirmed = true,      
                FirstName = registerDto.FirstName.Trim(),
                LastName = registerDto.LastName.Trim(),
                PhoneNumber = phone
            };

            foreach (var validator in _userManager.UserValidators)
            {
                var userCheck = await validator.ValidateAsync(_userManager, user);
                if (!userCheck.Succeeded)
                {
                    return userCheck;
                }
            }

            foreach (var validator in _userManager.PasswordValidators)
            {
                var passwordCheck = await validator.ValidateAsync(_userManager, user, registerDto.Password);
                if (!passwordCheck.Succeeded)
                {
                    return passwordCheck;
                }
            }

            var individual = registerDto.CustomerType == CustomerType.Individual;
            var customer = new Customer
            {
                Code = await GenerateCustomerCodeAsync(),
                Title = individual ? $"{user.FirstName} {user.LastName}" : registerDto.CompanyTitle!.Trim(),
                CustomerType = registerDto.CustomerType,
                TaxNumber = taxNumber,
                TaxOffice = individual ? "-" : registerDto.TaxOffice!.Trim(),
                Email = email,
                PhoneNumber = phone,
                Status = individual ? AccountStatus.Active : AccountStatus.PendingApproval,
                CountryId = country.Id
            };
            await _customerRepository.CreateAsync(customer);

            user.CustomerId = customer.Id;
            var created = await _userManager.CreateAsync(user, registerDto.Password);
            if (!created.Succeeded)
            {
                await _customerRepository.HardDeleteAsync(customer); // geri alma: gerçek silme  

                return created;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Customer);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                await _customerRepository.HardDeleteAsync(customer); // geri alma: gerçek silme 
                return roleResult;
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return IdentityResult.Success;
        }

        public async Task<ProfileDto?> GetProfileAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Lazy loading: ülke, şube ve cari ihtiyaç anında yüklenir
            return new ProfileDto
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Roles = roles.ToList(),
                CountryName = user.Country?.Name,
                BranchName = user.Branch?.Name,
                CustomerCode = user.Customer?.Code,
                CustomerTitle = user.Customer?.Title,
                CustomerType = user.Customer?.CustomerType,
                CustomerStatus = user.Customer?.Status
            };
        }

        private async Task<string> GenerateCustomerCodeAsync()
        {
            string code;
            do
            {
                code = $"CUK-{DateTime.UtcNow:yyMMdd}-{Random.Shared.Next(10000, 100000)}";
            }
            while (await _customerRepository.CodeExistsAsync(code));

            return code;
        }

        public async Task<UserCardDto?> GetUserCardAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserCardDto
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Initials = FirstLetter(user.FirstName) + FirstLetter(user.LastName),
                Role = RoleNames.All.FirstOrDefault(roles.Contains) ?? string.Empty,
                IsAdmin = roles.Contains(RoleNames.Admin),
                IsManager = roles.Contains(RoleNames.Manager),
                IsBranchStaff = RoleNames.BranchPanelRoles.Any(roles.Contains)
            };

        }


        // Kod = resx anahtarı: controller hatayı bu koda bakarak doğru alana yazar
        private IdentityResult Fail(string key) =>
            IdentityResult.Failed(new IdentityError { Code = key, Description = _localizer[key].Value });

        private static string FirstLetter(string? value) =>
            string.IsNullOrWhiteSpace(value) ? string.Empty : char.ToUpperInvariant(value.Trim()[0]).ToString();


    }
}
