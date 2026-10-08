using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Deliveries
{
    public class DeliveryRepository(AppDbContext context) : GenericRepository<Delivery>(context), IDeliveryRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Delivery?> GetByCargoAsync(Guid cargoId)
        {
            return await _context.Deliveries
                .FirstOrDefaultAsync(d => d.CargoId == cargoId);
        }
    }
}
