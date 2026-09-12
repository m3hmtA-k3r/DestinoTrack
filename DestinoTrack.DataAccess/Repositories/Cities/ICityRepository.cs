using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public interface ICityRepository : IRepository<City>
    {
        Task<List<City>> GetAllWithCountryAsync();
    }
}
