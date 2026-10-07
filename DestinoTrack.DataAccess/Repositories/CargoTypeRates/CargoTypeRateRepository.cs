using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.CargoTypeRates
{
    public class CargoTypeRateRepository(AppDbContext context) : GenericRepository<CargoTypeRate>(context), ICargoTypeRateRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<CargoTypeRate?> GetByTypeAsync(CargoType cargoType)
        {
            return await _context.CargoTypeRates
                .FirstOrDefaultAsync(r => r.CargoType == cargoType);
        }

        public async Task<List<CargoTypeRate>> GetAllOrderedAsync()
        {
            return await _context.CargoTypeRates
                .OrderBy(r => r.CargoType)
                .ToListAsync();
        }
    }
}
