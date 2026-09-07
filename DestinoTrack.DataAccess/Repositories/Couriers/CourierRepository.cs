using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Couriers
{
    public class CourierRepository(AppDbContext context) : GenericRepository<Courier>(context), ICourierRepository
    {
    }
}
