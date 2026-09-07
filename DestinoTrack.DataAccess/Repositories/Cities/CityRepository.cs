using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public  class CityRepository(AppDbContext context) : GenericRepository<City>(context), ICityRepository
    {
    }
}
