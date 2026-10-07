using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.CargoPrices
{
    public class CargoPriceRepository(AppDbContext context) : GenericRepository<CargoPrice>(context), ICargoPriceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<CargoPrice?> GetByCountryAndScopeAsync(Guid countryId, RouteScope routeScope)
        {
            // Tekil indeks sayesinde en fazla bir satır döner
            return await _context.CargoPrices
                .FirstOrDefaultAsync(p => p.CountryId == countryId && p.RouteScope == routeScope);
        }

        public async Task<List<CargoPrice>> GetAllWithCountryAsync()
        {
            return await _context.CargoPrices
                .Include(p => p.Country)
                .OrderBy(p => p.Country.Name)
                .ThenBy(p => p.RouteScope)
                .ToListAsync();
        }
    }
}
