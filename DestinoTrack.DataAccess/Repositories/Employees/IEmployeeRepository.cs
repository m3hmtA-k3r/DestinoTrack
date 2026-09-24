using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.Employees
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        // Sayfalı liste: görev ve ad/soyad/telefon araması isteğe bağlı · tesis ve şehir adıyla birlikte
        Task<(List<Employee> Items, int TotalCount)> GetPagedWithBranchAsync(EmployeeJobType? jobType, string? search, int page, int pageSize);

        // Görev çiplerindeki sayılar: EmployeeJobType → o görevdeki personel sayısı
        Task<Dictionary<EmployeeJobType, int>> GetCountsByJobTypeAsync();

        // Silme öncesi: kurye olarak taşıdığı kargo ya da tahsil ettiği ödeme var mı
        Task<bool> HasDependentsAsync(Guid employeeId);

        //hesabın personel kaydı var mı — varsa şubesi kullanıcı ekranından değil Personel ekranından yönetilir
        Task<bool> HasEmployeeAsync(Guid appUserId);

    }
}
