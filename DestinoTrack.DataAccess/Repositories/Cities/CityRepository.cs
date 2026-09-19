using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public class CityRepository(AppDbContext context) : GenericRepository<City>(context), ICityRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<(List<City> Items, int TotalCount)> GetPagedWithCountryAsync(Guid? countryId, string? search, int page, int pageSize)
        {
            // Filtreler eskisiyle aynı — değişen yalnızca sonunda sayfanın alınması
            IQueryable<City> query = _context.Cities.Include(c => c.Country);

            if (countryId.HasValue)
            {
                query = query.Where(c => c.CountryId == countryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.Name.Contains(term));
            }

            // Sayfa sayısı filtreye uyan TÜM kayıtlara göre hesaplanır → önce sayım   
            var totalCount = await query.CountAsync();

            // Skip / Take veritabanında çalışır: 3. sayfa istendiğinde yalnızca 20 satır gelir
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }


        // CountryId — sayım veritabanında yapılır, şehirler belleğe çekilmez
        public async Task<Dictionary<Guid, int>> GetCityCountsByCountryAsync()
        {
            return await _context.Cities
                .GroupBy(c => c.CountryId)
                .Select(g => new { CountryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CountryId, x => x.Count);
        }


        // Soft delete'te veritabanının FK koruması çalışmaz — kural serviste bu sorguyla işler
        public async Task<bool> HasBranchesAsync(Guid cityId)
        {
            return await _context.Branches.AnyAsync(b => b.CityId == cityId);
        }

    }
}
