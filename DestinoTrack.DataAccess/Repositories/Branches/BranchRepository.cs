using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Branches
{
    public class BranchRepository(AppDbContext _context) : GenericRepository<Branch>(_context), IBranchRepository
    {
    }
}
