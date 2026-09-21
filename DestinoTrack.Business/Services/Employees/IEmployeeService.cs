using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.EmployeeDtos;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<PagedResult<ResultEmployeeDto>> GetPagedAsync(EmployeeJobType? jobType = null, string? search = null, int page = 1);

        // Görev çipleri: dört görev + her birinin sayısı (0 olanlar dahil)
        Task<List<EmployeeJobTypeFilterDto>> GetJobTypeFiltersAsync();

        Task<UpdateEmployeeDto> GetByIdAsync(Guid id);

        // Hesap da açılıyorsa Identity hataları (aynı e-posta, şifre kuralı) forma dönmek üzere döner
        Task<IdentityResult> CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto);

        // pasifleştirme bağlı hesabı da kilitler, aktifleştirme açar
        Task SetActiveAsync(Guid id, bool isActive);

        // bağlı kargo/ödeme yoksa silinir; bağlı hesap kilitlenir
        Task DeleteAsync(Guid id);
    }
}
