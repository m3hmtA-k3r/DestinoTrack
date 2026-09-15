using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Customers
{
    public class CustomerRepository(AppDbContext context) : GenericRepository<Customer>(context), ICustomerRepository
    {
        private readonly AppDbContext _context = context;

        // AnyAsync: kayıt belleğe çekilmez, veritabanına yalnızca "var mı?" sorusu gider
        public async Task<bool> TaxNumberExistsAsync(Guid countryId, string taxNumber)
        {
            return await _context.Customers
                .AnyAsync(c => c.CountryId == countryId && c.TaxNumber == taxNumber);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Customers
                .AnyAsync(c => c.Code == code); 
        }
    }
}
