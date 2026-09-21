using DestinoTrack.Business.Consts;
using DestinoTrack.DataAccess.Repositories.Employees;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Employees
{
    public class EmployeeService(IEmployeeRepository _employeeRepository,
                                 UserManager<AppUser> _userManager,
                                 IdentityErrorDescriber _errorDescriber,
                                 IStringLocalizer<SharedResource> _localizer) : IEmployeeService
    {
        // Kullanıcılar ekranıyla aynı: pasif hesap = süresiz kilit
        private static readonly DateTimeOffset DeactivatedUntil = DateTimeOffset.MaxValue;

        public async Task<PagedResult<ResultEmployeeDto>> GetPagedAsync(EmployeeJobType? jobType = null, string? search = null, int page = 1)
        {
            page = page < 1 ? 1 : page;

            var (employees, totalCount) = await _employeeRepository.GetPagedWithBranchAsync(jobType, search, page, Paging.PageSize);

            // Son sayfadaki tek kayıt silinince o sayfa boş kalır → varsa yeni son sayfa gösterilir
            if (employees.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (employees, totalCount) = await _employeeRepository.GetPagedWithBranchAsync(jobType, search, page, Paging.PageSize);
            }

            return new PagedResult<ResultEmployeeDto>
            {
                Items = employees.Select(e => new ResultEmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    PhoneNumber = e.PhoneNumber,
                    IsActive = e.IsActive,
                    JobType = e.JobType,
                    BranchId = e.BranchId,
                    BranchName = e.Branch?.Name,
                    CityName = e.Branch?.City?.Name,
                    VehicleType = e.VehicleType,
                    VehiclePlate = e.VehiclePlate,
                    Rating = e.Rating,
                    HasLoginAccount = e.AppUserId != null
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<EmployeeJobTypeFilterDto>> GetJobTypeFiltersAsync()
        {
            var counts = await _employeeRepository.GetCountsByJobTypeAsync();

            return Enum.GetValues<EmployeeJobType>()
                .Select(t => new EmployeeJobTypeFilterDto { JobType = t, EmployeeCount = counts.GetValueOrDefault(t) })
                .ToList();
        }

        public async Task<UpdateEmployeeDto> GetByIdAsync(Guid id)
        {
            var employee = await FindAsync(id);

            var dto = employee.Adapt<UpdateEmployeeDto>();
            dto.HasLoginAccount = employee.AppUserId != null;
            return dto;
        }

        public async Task<IdentityResult> CreateAsync(CreateEmployeeDto createEmployeeDto)
        {
            var email = createEmployeeDto.Email?.Trim();

            // Aynı e-postayla hesap varsa hiçbir kayıt oluşturulmadan forma dönülür
            if (createEmployeeDto.CreateAccount && await _userManager.FindByEmailAsync(email!) != null)
            {
                return IdentityResult.Failed(_errorDescriber.DuplicateEmail(email!));
            }

            var employee = createEmployeeDto.Adapt<Employee>();
            employee.IsActive = true;
            ClearCourierFieldsIfNotCourier(employee);
            await _employeeRepository.CreateAsync(employee);

            if (!createEmployeeDto.CreateAccount)
            {
                return IdentityResult.Success;
            }

            //hesap personelden sonra açılır; bir adım başarısız olursa personel de geri alınır ( kayıt formundaki kalıp)
            var user = new AppUser
            {
                UserName = email,                  // giriş e-postayla yapılır
                Email = email,
                EmailConfirmed = true,             // hesabı Admin açtığı için doğrulanmış sayılır
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                BranchId = employee.BranchId       // şube paneli bu alanla süzecek
            };

            var result = await _userManager.CreateAsync(user, createEmployeeDto.Password!);
            if (!result.Succeeded)
            {
                await _employeeRepository.HardDeleteAsync(employee);
                return result;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, createEmployeeDto.AccountRole!);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);          // rolsüz hesap kalmasın
                await _employeeRepository.HardDeleteAsync(employee);
                return roleResult;
            }

            employee.AppUserId = user.Id;
            await _employeeRepository.UpdateAsync(employee);

            return IdentityResult.Success;
        }

        public async Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await FindAsync(updateEmployeeDto.Id);

            // Mevcut kayda yazılır: IsActive · AppUserId · Rating · CreatedDate korunur (DTO'da yoklar)
            updateEmployeeDto.Adapt(employee);
            ClearCourierFieldsIfNotCourier(employee);
            await _employeeRepository.UpdateAsync(employee);

            // Bağlı hesap personelle uyumlu kalır: ad · soyad · telefon · şube
            var user = await FindUserAsync(employee.AppUserId);
            if (user != null)
            {
                user.FirstName = employee.FirstName;
                user.LastName = employee.LastName;
                user.PhoneNumber = employee.PhoneNumber;
                user.BranchId = employee.BranchId;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task SetActiveAsync(Guid id, bool isActive)
        {
            var employee = await FindAsync(id);
            employee.IsActive = isActive;
            await _employeeRepository.UpdateAsync(employee);

            await SetAccountLockAsync(employee.AppUserId, locked: !isActive);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await FindAsync(id);

            // kurye olarak taşıdığı kargo ya da tahsil ettiği ödeme varsa silinmez
            if (await _employeeRepository.HasDependentsAsync(id))
            {
                throw new ValidationException(_localizer["EmployeeHasDependents"].Value);
            }

            await _employeeRepository.DeleteAsync(employee);

            // Hesap silinmez (işlem geçmişi ona bağlı olabilir) ama açık da kalmaz: kilitlenir
            await SetAccountLockAsync(employee.AppUserId, locked: true);
        }

        // Kullanıcılar ekranındaki pasifleştirmenin aynısı: süresiz kilit + güvenlik damgası (açık oturum düşer)
        private async Task SetAccountLockAsync(Guid? appUserId, bool locked)
        {
            var user = await FindUserAsync(appUserId);
            if (user == null)
            {
                return;
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, locked ? DeactivatedUntil : null);

            if (locked)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
        }

        private async Task<AppUser?> FindUserAsync(Guid? appUserId)
        {
            return appUserId == null ? null : await _userManager.FindByIdAsync(appUserId.Value.ToString());
        }

        private async Task<Employee> FindAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new ValidationException(_localizer["EmployeeNotFound"].Value);
            }
            return employee;
        }

        // kurye olmayan personelde araç bilgisi tutulmaz (görev değişince eski plaka kalmasın)
        private static void ClearCourierFieldsIfNotCourier(Employee employee)
        {
            if (employee.JobType != EmployeeJobType.Courier)
            {
                employee.VehicleType = null;
                employee.VehiclePlate = null;
                employee.Region = null;
            }
        }
    }
}
