using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Cities
{
    public class CityRepository(AppDbContext context) : GenericRepository<City>(context), ICityRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<City>> GetAllWithCountryAsync(Guid? countryId = null, string? search = null)
        {
            // Sorgu adım adım kurulur; ToListAsync'e kadar veritabanına gidilmez
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

            return await query
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // CountryId — sayım veritabanında yapılır, şehirler belleğe çekilmez
        public async Task<Dictionary<Guid, int>> GetCityCountsByCountryAsync()
        {
            return await _context.Cities
                .GroupBy(c => c.CountryId)
                .Select(g => new { CountryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CountryId, x => x.Count);
        }
    }
}
