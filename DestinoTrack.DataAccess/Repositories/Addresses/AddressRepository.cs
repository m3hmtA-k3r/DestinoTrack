using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Addresses
{
    public class AddressRepository(AppDbContext _context) : GenericRepository<Address>(_context), IAddressRepository
    {
    }
}
