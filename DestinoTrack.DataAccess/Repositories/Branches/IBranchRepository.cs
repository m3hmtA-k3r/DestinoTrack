using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.Branches
{
    public interface IBranchRepository : IRepository<Branch>
    {
        // Sayfalı liste  tür ve ad/kod araması isteğe bağlı · şehir ve ülke adıyla birlikte
        Task<(List<Branch> Items, int TotalCount)> GetPagedWithCityAsync(BranchType? branchType, string? search, int page, int pageSize);

        // Tek kayıt: düzenleme ekranı için şehriyle birlikte
        Task<Branch?> GetWithCityAsync(Guid id);

        // Tür çiplerindeki sayılar: BranchType → o türdeki şube sayısı
        Task<Dictionary<BranchType, int>> GetCountsByTypeAsync();

        // Silme öncesi: bağlı kargo · personel · ödeme · şube müdürü var mı
        Task<bool> HasDependentsAsync(Guid branchId);

        // Açılır listeler: sayfasız, ada göre sıralı, şehriyle birlikte (ülke şehirden gelir)
        Task<List<Branch>> GetLookupAsync();
        // VC kartı: kapsamdaki tesis ve personel toplamları (null = tüm tesisler)
        Task<(int EmployeeCount, int CourierCount, int TotalCapacity, int TotalDockCount)> GetSummaryAsync(BranchType? branchType);

    }
}
