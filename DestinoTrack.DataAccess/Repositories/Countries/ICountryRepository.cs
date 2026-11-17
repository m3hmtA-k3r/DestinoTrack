using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Countries
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<bool> HasDependentsAsync(Guid countryId);

        // Sayfalı liste: ada göre sıralı, yalnızca istenen sayfa + toplam sayı
        Task<(List<Country> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);

        // Ülke paneli başlığı: ülkenin adı + o ülkedeki şube sayısı (tek sorgu).
        // Ülke bulunamazsa null — "ülkesi atanmamış/silinmiş Manager" hâlini servis buradan anlar
        Task<(string Name, int BranchCount)?> GetPanelHeaderAsync(Guid countryId);

    }
}
