using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public class CityRepository(AppDbContext context) : GenericRepository<City>(context), ICityRepository
    {
        private readonly AppDbContext _context = context;

        // Listeleme ekranında ülke adı da gösteriliyor.
        // Include olmadan her satır için ayrı sorgu açılırdı (N+1).
        public async Task<List<City>> GetAllWithCountryAsync()
        {
            return await _context.Cities
                .Include(c => c.Country)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
