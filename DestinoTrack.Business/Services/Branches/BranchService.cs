using DestinoTrack.Business.Consts;
using DestinoTrack.DataAccess.Repositories.Branches;
using DestinoTrack.DTO.DTOs.BranchDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Mapster;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Branches
{
    public class BranchService(IBranchRepository _branchRepository, IStringLocalizer<SharedResource> _localizer) : IBranchService
    {
        public async Task<PagedResult<ResultBranchDto>> GetPagedAsync(BranchType? branchType = null, string? search = null, int page = 1)
        {
            page = page < 1 ? 1 : page;

            var (branches, totalCount) = await _branchRepository.GetPagedWithCityAsync(branchType, search, page, Paging.PageSize);

            // Son sayfadaki tek kayıt silinince o sayfa boş kalır → varsa yeni son sayfa gösterilir
            if (branches.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (branches, totalCount) = await _branchRepository.GetPagedWithCityAsync(branchType, search, page, Paging.PageSize);
            }

            return new PagedResult<ResultBranchDto>
            {
                Items = branches.Select(b => new ResultBranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    BranchType = b.BranchType,
                    CityId = b.CityId,
                    CityName = b.City?.Name,
                    CountryName = b.City?.Country?.Name,
                    Capacity = b.Capacity,
                    DockCount = b.DockCount
                }).ToList(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
        }

        // Hiç şubesi olmayan tür de çip olarak görünür (sayısı 0) — bu yüzden sayımlar enum listesinin üzerine eklenir
        public async Task<List<BranchTypeFilterDto>> GetTypeFiltersAsync()
        {
            var counts = await _branchRepository.GetCountsByTypeAsync();

            return Enum.GetValues<BranchType>()
                .Select(t => new BranchTypeFilterDto
                {
                    BranchType = t,
                    BranchCount = counts.GetValueOrDefault(t)
                })
                .ToList();
        }

        public async Task<List<BranchLookupDto>> GetLookupAsync()
        {
            var branches = await _branchRepository.GetLookupAsync();

            return branches.Select(b => new BranchLookupDto
            {
                Id = b.Id,
                Name = b.Name,
                CityName = b.City?.Name,
                CountryId = b.City?.CountryId ?? Guid.Empty
            }).ToList();
        }

        public async Task<UpdateBranchDto> GetByIdAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new ValidationException(_localizer["BranchNotFound"].Value);
            }

            return branch.Adapt<UpdateBranchDto>();
        }

        public async Task CreateAsync(CreateBranchDto createBranchDto)
        {
            var branch = createBranchDto.Adapt<Branch>();
            await _branchRepository.CreateAsync(branch);
        }

        public async Task UpdateAsync(UpdateBranchDto updateBranchDto)
        {
            var branch = await _branchRepository.GetByIdAsync(updateBranchDto.Id);
            if (branch == null)
            {
                throw new ValidationException(_localizer["BranchNotFound"].Value);
            }

            // Mevcut kayıt üzerine yazılır: CreatedDate ve CurrentLoad korunur
            updateBranchDto.Adapt(branch);
            await _branchRepository.UpdateAsync(branch);
        }

        public async Task DeleteAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new ValidationException(_localizer["BranchNotFound"].Value);
            }

            // bağlı kargo · personel · ödeme · müdür varken silinmez
            // (soft delete'te veritabanının FK koruması çalışmıyor)
            if (await _branchRepository.HasDependentsAsync(id))
            {
                throw new ValidationException(_localizer["BranchHasDependents"].Value);
            }

            await _branchRepository.DeleteAsync(branch);
        }

        public async Task<BranchSummaryDto> GetSummaryAsync(BranchType? branchType)
        {
            var (employeeCount, courierCount, totalCapacity, totalDockCount) = await _branchRepository.GetSummaryAsync(branchType);

            return new BranchSummaryDto
            {
                BranchType = branchType,
                EmployeeCount = employeeCount,
                CourierCount = courierCount,
                TotalCapacity = totalCapacity,
                TotalDockCount = totalDockCount
            };
        }

    }
}
