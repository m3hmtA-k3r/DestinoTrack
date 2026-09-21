using DestinoTrack.DTO.DTOs.BranchDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.Branches
{
    public interface IBranchService
    {
        // Sayfalı liste: tür filtresi ve arama isteğe bağlı
        Task<PagedResult<ResultBranchDto>> GetPagedAsync(BranchType? branchType = null, string? search = null, int page = 1);

        // Tür çipleri: dört tesis türü + her birinin sayısı (0 olanlar dahil)
        Task<List<BranchTypeFilterDto>> GetTypeFiltersAsync();

        // Formlardaki tesis açılır listesi (sayfalama yok)
        Task<List<BranchLookupDto>> GetLookupAsync();

        Task<UpdateBranchDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateBranchDto createBranchDto);
        Task UpdateAsync(UpdateBranchDto updateBranchDto);
        Task DeleteAsync(Guid id);
    }
}
