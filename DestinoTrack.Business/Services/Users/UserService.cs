using DestinoTrack.Business.Consts;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.UserDtos;
using DestinoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Users
{
    public class UserService(UserManager<AppUser> _userManager,
                             ICountryRepository _countryRepository,
                             IdentityErrorDescriber _errorDescriber,
                             IStringLocalizer<SharedResource> _localizer) : IUserService
    {
        // pasif kullanıcı = süresiz kilitli
        private static readonly DateTimeOffset DeactivatedUntil = DateTimeOffset.MaxValue;
        public async Task<PagedResult<ResultUserDto>> GetPagedAsync(string? role = null, string? search = null, int page = 1)
        {
            page = page < 1 ? 1 : page;

            var staffRoles = await GetStaffRolesAsync();

            var ids = staffRoles
                .Where(r => string.IsNullOrEmpty(role) || r.Value == role)
                .Select(r => r.Key)
                .ToList();

            var query = _userManager.Users.Where(u => ids.Contains(u.Id));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u => u.FirstName.Contains(term) || u.LastName.Contains(term) || u.Email!.Contains(term));
            }

            var totalCount = await query.CountAsync();

            // Sayım burada yapıldığı için son sayfa düzeltmesi tek sorguyla olur
            var totalPages = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            // Ülke ve şube adı aynı sorguda gelir — kullanıcı başına ayrı sorgu atılmaz
            var users = await query
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Skip((page - 1) * Paging.PageSize)
                .Take(Paging.PageSize)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    CountryName = u.Country.Name,
                    BranchName = u.Branch.Name,
                    u.LockoutEnd
                })
                .ToListAsync();

            return new PagedResult<ResultUserDto>
            {
                Items = users.Select(u => new ResultUserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email!,
                    Role = staffRoles[u.Id],
                    CountryName = u.CountryName,
                    BranchName = u.BranchName,
                    IsActive = u.LockoutEnd != DeactivatedUntil
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }


        public async Task<Dictionary<string, int>> GetRoleCountsAsync()
        {
            var staffRoles = await GetStaffRolesAsync();

            // Kullanıcısı olmayan rol de 0 ile gelir
            return RoleNames.Staff.ToDictionary(r => r, r => staffRoles.Values.Count(v => v == r));
        }

        public async Task<UpdateUserDto> GetByIdAsync(Guid id)
        {
            var (user, role) = await FindStaffAsync(id);

            return new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = role,
                CountryId = user.CountryId,
                BranchId = user.BranchId
            };
        }

        public async Task<IdentityResult> CreateAsync(CreateUserDto createUserDto)
        {
            var check = await CheckRoleAndCountryAsync(createUserDto.Role, createUserDto.CountryId);
            if (!check.Succeeded)
            {
                return check;
            }

            var email = createUserDto.Email.Trim();
            var user = new AppUser
            {
                UserName = email,          // Giris e-postayla yapılır
                Email = email,
                EmailConfirmed = true,     // Hesabı Admin açtığı için doğrulanmıs sayılır
                FirstName = createUserDto.FirstName.Trim(),
                LastName = createUserDto.LastName.Trim(),
                CountryId = createUserDto.Role == RoleNames.Manager ? createUserDto.CountryId : null
                 
            };

            // Şifre ve e-posta tekilliği burada denetlenir — hata varsa kayıt oluşmaz
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, createUserDto.Role);
            if (!roleResult.Succeeded)
            {
                // Rolsüz hesap kalmasın
                await _userManager.DeleteAsync(user);
                return roleResult;
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(UpdateUserDto updateUserDto, Guid currentUserId)
        {
            var (user, currentRole) = await FindStaffAsync(updateUserDto.Id);

            var check = await CheckRoleAndCountryAsync(updateUserDto.Role, updateUserDto.CountryId);
            if (!check.Succeeded)
            {
                return check;
            }

            if (updateUserDto.Role != currentRole)
            {
                // kendi rolünü değiştiremez · son aktif Admin'in rolü değişemez
                if (user.Id == currentUserId)
                {
                    return Fail("CannotChangeOwnRole");
                }
                if (currentRole == RoleNames.Admin && await IsLastActiveAdminAsync(user.Id))
                {
                    return Fail("LastAdminProtected");
                }
            }

            // Önce hepsini denetle, sonra yaz —> bir adım hata verirse yarım güncelleme kalmasın
            var email = updateUserDto.Email.Trim();
            var owner = await _userManager.FindByEmailAsync(email);
            if (owner != null && owner.Id != user.Id)
            {
                return IdentityResult.Failed(_errorDescriber.DuplicateEmail(email));
            }

            var changePassword = !string.IsNullOrEmpty(updateUserDto.NewPassword);
            if (changePassword)
            {
                foreach (var validator in _userManager.PasswordValidators)
                {
                    var passwordCheck = await validator.ValidateAsync(_userManager, user, updateUserDto.NewPassword);
                    if (!passwordCheck.Succeeded)
                    {
                        return passwordCheck;
                    }
                }
            }

            user.FirstName = updateUserDto.FirstName.Trim();
            user.LastName = updateUserDto.LastName.Trim();
            user.Email = email;
            user.UserName = email;
            user.CountryId = updateUserDto.Role == RoleNames.Manager ? updateUserDto.CountryId : null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }

            if (updateUserDto.Role != currentRole)
            {
                await _userManager.RemoveFromRoleAsync(user, currentRole);
                await _userManager.AddToRoleAsync(user, updateUserDto.Role);
            }

            if (changePassword)
            {
                // Eski şifreyi bilmeden yeni şifre: sıfırlama anahtarıyla (AddDefaultTokenProviders)
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                result = await _userManager.ResetPasswordAsync(user, token, updateUserDto.NewPassword!);
            }

            return result;
        }

        public async Task<IdentityResult> SetActiveAsync(Guid id, bool isActive, Guid currentUserId)
        {
            var (user, role) = await FindStaffAsync(id);

            if (!isActive)
            {
               
                if (user.Id == currentUserId)
                {
                    return Fail("CannotDeactivateSelf");
                }
                if (role == RoleNames.Admin && await IsLastActiveAdminAsync(user.Id))
                {
                    return Fail("LastAdminProtected");
                }
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            var result = await _userManager.SetLockoutEndDateAsync(user, isActive ? null : DeactivatedUntil);
            if (!result.Succeeded)
            {
                return result;
            }

            if (isActive)
            {
                // Pasifken birikmiş hatalı deneme sayısı sıfırlanır
                await _userManager.ResetAccessFailedCountAsync(user);
            }
            else
            {
                // Güvenlik damgası değişir — açık kalmış oturum da düşer
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return IdentityResult.Success;
        }

        // Personel kullanıcı → rolü (rol başına tek sorgu; Customer bu listeye girmez)
        private async Task<Dictionary<Guid, string>> GetStaffRolesAsync()
        {
            var staffRoles = new Dictionary<Guid, string>();
            foreach (var role in RoleNames.Staff)
            {
                foreach (var user in await _userManager.GetUsersInRoleAsync(role))
                {
                    staffRoles[user.Id] = role;
                }
            }
            return staffRoles;
        }

        // Bulunamayan ve personel olmayan (Customer) kullanıcı için aynı mesaj — bu ekran müşteri yönet3mez
        private async Task<(AppUser User, string Role)> FindStaffAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(r => RoleNames.Staff.Contains(r));
                if (role != null)
                {
                    return (user, role);
                }
            }

            throw new ValidationException(_localizer["UserNotFound"].Value);
        }

        // Validator'ı atlayan bir çağrıya karşı ikinci kontrol & ülkenin gerçekten var olması
        private async Task<IdentityResult> CheckRoleAndCountryAsync(string role, Guid? countryId)
        {
            if (!RoleNames.Staff.Contains(role))
            {
                return Fail("RoleInvalid");
            }

            if (role == RoleNames.Manager)
            {
                if (countryId == null)
                {
                    return Fail("CountryRequiredForManager");
                }
                if (await _countryRepository.GetByIdAsync(countryId.Value) == null)
                {
                    return Fail("CountryNotFound");
                }
            }

            return IdentityResult.Success;
        }

        // Bu kullanıcı dışında aktif Admin kalmıyor mu?
        private async Task<bool> IsLastActiveAdminAsync(Guid userId)
        {
            var admins = await _userManager.GetUsersInRoleAsync(RoleNames.Admin);
            return !admins.Any(a => a.Id != userId && a.LockoutEnd != DeactivatedUntil);
        }

        public async Task<string> GetLockedOutMessageAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.LockoutEnd != DeactivatedUntil)
            {
                //  5 hatalı denemenin 15 dakikalık kilidi
                return _localizer["AccountLocked"].Value;
            }

            // pasif hesap yalnızca şifre doğruysa söylenir — yanlış şifreyle hesabın durumu öğrenilemez
            return await _userManager.CheckPasswordAsync(user, password)
                ? _localizer["AccountDeactivated"].Value
                : _localizer["InvalidLogin"].Value;
        }


        //resx anahtarı: controller hatayı bu koda bakarak doğru alana yazacak
        private IdentityResult Fail(string key) =>
            IdentityResult.Failed(new IdentityError { Code = key, Description = _localizer[key].Value });
    }
}
