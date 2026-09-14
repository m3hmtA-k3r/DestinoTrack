using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public interface ICityRepository : IRepository<City>
    {
        // Ülke ve ad filtresi isteğe bağlı 
        Task<List<City>> GetAllWithCountryAsync(Guid? countryId = null, string? search = null);

        // Ülke çiplerindeki sayılar: CountryId → o ülkedeki şehir sayısı
        Task<Dictionary<Guid, int>> GetCityCountsByCountryAsync();
    }
}
