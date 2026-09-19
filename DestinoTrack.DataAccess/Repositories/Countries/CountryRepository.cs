using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Countries
{
    public class CountryRepository(AppDbContext context) : GenericRepository<Country>(context), ICountryRepository
    {
        private readonly AppDbContext _context = context;

        // Soft delete'te veritabanının FK koruması çalışmaz — kural serviste bu sorguyla işler. Silinmiş kayıtlar sayılmaz
        public async Task<bool> HasDependentsAsync(Guid countryId)
        {
            return await _context.Cities.AnyAsync(c => c.CountryId == countryId)
                || await _context.Customers.AnyAsync(m => m.CountryId == countryId)
                || await _context.Users.AnyAsync(u => u.CountryId == countryId);
        }


        public async Task<(List<Country> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            IQueryable<Country> query = _context.Countries;

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}
