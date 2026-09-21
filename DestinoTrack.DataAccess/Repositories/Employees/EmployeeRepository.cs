using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Employees
{
    public class EmployeeRepository(AppDbContext context) : GenericRepository<Employee>(context), IEmployeeRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<(List<Employee> Items, int TotalCount)> GetPagedWithBranchAsync(EmployeeJobType? jobType, string? search, int page, int pageSize)
        {
            // Branch.City: listede "reyhanlı Şubesi · Hatay" yazılacak
            IQueryable<Employee> query = _context.Employees
                .Include(e => e.Branch)
                    .ThenInclude(b => b.City);

            if (jobType.HasValue)
            {
                query = query.Where(e => e.JobType == jobType.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(e => e.FirstName.Contains(term) || e.LastName.Contains(term) || e.PhoneNumber.Contains(term));
            }

            var totalCount = await query.CountAsync();

            // Aktifler önce, sonra ada göre: pasif personel listenin sonunda soluk görünür
            var items = await query
                .OrderByDescending(e => e.IsActive)
                .ThenBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Dictionary<EmployeeJobType, int>> GetCountsByJobTypeAsync()
        {
            return await _context.Employees
                .GroupBy(e => e.JobType)
                .Select(g => new { JobType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.JobType, x => x.Count);
        }

        // Soft delete'te veritabanının FK koruması çalışmaz — kural serviste bu sorguyla işler
        public async Task<bool> HasDependentsAsync(Guid employeeId)
        {
            return await _context.Cargos.AnyAsync(c => c.CourierId == employeeId)
                || await _context.Payments.AnyAsync(p => p.CollectedByCourierId == employeeId);
        }
    }
}
