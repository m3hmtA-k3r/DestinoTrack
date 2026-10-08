using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.DeliveryExceptions
{
    public class DeliveryExceptionRepository(AppDbContext context) : GenericRepository<DeliveryException>(context), IDeliveryExceptionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<int> CountByCargoAsync(Guid cargoId)
        {
            return await _context.DeliveryExceptions
                .CountAsync(x => x.CargoId == cargoId);
        }
    }
}
