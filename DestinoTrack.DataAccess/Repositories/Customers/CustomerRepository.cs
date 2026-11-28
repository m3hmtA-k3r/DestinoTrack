using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Customers
{
    public class CustomerRepository(AppDbContext context) : GenericRepository<Customer>(context), ICustomerRepository
    {
        private readonly AppDbContext _context = context;

        // AnyAsync: kayıt belleğe çekilmez, veritabanına yalnızca "var mı?" sorusu gider
        public async Task<bool> TaxNumberExistsAsync(Guid countryId, string taxNumber, Guid? excludeCustomerId = null)
        {
            return await _context.Customers
                .AnyAsync(c => c.CountryId == countryId
                               && c.TaxNumber == taxNumber
                               && (excludeCustomerId == null || c.Id != excludeCustomerId.Value));
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Customers
                .AnyAsync(c => c.Code == code); 
        }
        public async Task<List<Customer>> GetActiveByCountryAsync(Guid countryId)
        {
            return await _context.Customers
                .Where(c => c.Status == AccountStatus.Active && c.CountryId == countryId)
                .OrderBy(c => c.Title)
                .ToListAsync();
        }

        public async Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(AccountStatus? status, CustomerType? customerType,
                                                                                string? search, int page, int pageSize)
        {
            // Country: listede ülke sütunu var. Include olmazsa lazy loading her satır için sorgu açar
            IQueryable<Customer> query = _context.Customers.Include(c => c.Country);

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (customerType.HasValue)
            {
                query = query.Where(c => c.CustomerType == customerType.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.Title.Contains(term) || c.Code.Contains(term) || c.TaxNumber.Contains(term));
            }

            var totalCount = await query.CountAsync();

            // Kod tekil: eşit ünvanlarda sıra sayfalar arasında kaymasın
            var items = await query
                .OrderByDescending(c => c.Status == AccountStatus.PendingApproval)
                .ThenBy(c => c.Title)
                .ThenBy(c => c.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Dictionary<Guid, (int CargoCount, DateTime? LastCargoDate)>> GetCargoStatsAsync(List<Guid> customerIds)
        {
            if (customerIds.Count == 0)
            {
                // Boş sayfada veritabanına hiç gitme
                return new Dictionary<Guid, (int, DateTime?)>();
            }

            var liste = await _context.Cargos
                .Where(c => c.CustomerId != null && customerIds.Contains(c.CustomerId.Value))
                .GroupBy(c => c.CustomerId!.Value)
                .Select(g => new { Id = g.Key, Adet = g.Count(), Son = (DateTime?)g.Max(x => x.ShipmentDate) })
                .ToListAsync();

            return liste.ToDictionary(x => x.Id, x => (x.Adet, x.Son));
        }


        public async Task<Dictionary<AccountStatus, int>> GetCountsByStatusAsync()
        {
            return await _context.Customers
                .GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<(int ActiveCorporate, int NewThisMonth)> GetPortfolioCountsAsync()
        {
            // Ay sınırı UTC: kayıt tarihleri UTC tutuluyor, yerel saate çevirmek ayın ilk/son
            // gününde kaydedilen müşteriyi yanlış aya atar
            var simdi = DateTime.UtcNow;
            var ayBasi = new DateTime(simdi.Year, simdi.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // "Portföy" = kurumsal VE aktif. Bireysel hesaplar portföy sayılmaz
            var activeCorporate = await _context.Customers
                .CountAsync(c => c.CustomerType == CustomerType.Corporate && c.Status == AccountStatus.Active);

            var newThisMonth = await _context.Customers
                .CountAsync(c => c.CreatedDate >= ayBasi);

            return (activeCorporate, newThisMonth);
        }
    }
}
