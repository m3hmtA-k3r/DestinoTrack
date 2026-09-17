using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Countries
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<bool> HasDependentsAsync(Guid countryId);

    }
}
