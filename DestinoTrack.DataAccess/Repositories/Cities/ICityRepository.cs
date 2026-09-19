using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public interface ICityRepository : IRepository<City>
    {
        // Sayfalı liste : filtreye uyan kayıtların yalnızca istenen sayfası + toplam sayı
        Task<(List<City> Items, int TotalCount)> GetPagedWithCountryAsync(Guid? countryId, string? search, int page, int pageSize);

        // Ülke çiplerindeki sayılar: CountryId → o ülkedeki şehir sayısı
        Task<Dictionary<Guid, int>> GetCityCountsByCountryAsync();

        // Silme öncesi: bu şehre bağlı şube var mı
        Task<bool> HasBranchesAsync(Guid cityId);

    }
}
