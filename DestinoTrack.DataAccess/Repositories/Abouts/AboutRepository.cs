using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Abouts
{
    public class AboutRepository(AppDbContext _context) : GenericRepository<About>(_context), IAboutRepository
    {
    }
}
