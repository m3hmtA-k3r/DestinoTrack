using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.CargoMovements
{
    public class CargoMovementRepository(AppDbContext context) : GenericRepository<CargoMovement>(context), ICargoMovementRepository
    {
    }
}
