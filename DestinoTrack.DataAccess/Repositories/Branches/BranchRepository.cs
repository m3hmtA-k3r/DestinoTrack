using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Branches
{
    public class BranchRepository(AppDbContext context) : GenericRepository<Branch>(context), IBranchRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<(List<Branch> Items, int TotalCount)> GetPagedWithCityAsync(BranchType? branchType, string? search, int page, int pageSize)
        {
            // City.Country: listede ülke adı da gösterilecek
            IQueryable<Branch> query = _context.Branches
                .Include(b => b.City)
                    .ThenInclude(c => c.Country);

            if (branchType.HasValue)
            {
                query = query.Where(b => b.BranchType == branchType.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Ad, kod ve şehir adında arar
                var term = search.Trim();
                query = query.Where(b => b.Name.Contains(term) || b.Code.Contains(term) || b.City.Name.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Branch?> GetWithCityAsync(Guid id)
        {
            return await _context.Branches
                .Include(b => b.City)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Dictionary<BranchType, int>> GetCountsByTypeAsync()
        {
            // Sayım veritabanında yapılır, şubeler belleğe çekilmez
            return await _context.Branches
                .GroupBy(b => b.BranchType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }

        public async Task<bool> HasDependentsAsync(Guid branchId)
        {
            return await _context.Cargos.AnyAsync(c => c.OriginBranchId == branchId || c.DestinationBranchId == branchId)
                || await _context.Employees.AnyAsync(e => e.BranchId == branchId)
                || await _context.Payments.AnyAsync(p => p.CollectedByBranchId == branchId)
                || await _context.Branches.AnyAsync(b => b.Id == branchId && b.ManagerId != null);
        }

        public async Task<List<Branch>> GetLookupAsync()
        {
            return await _context.Branches
                .Include(b => b.City)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<(int EmployeeCount, int CourierCount, int TotalCapacity, int TotalDockCount)> GetSummaryAsync(BranchType? branchType)
        {
            IQueryable<Branch> branches = _context.Branches;
            IQueryable<Employee> employees = _context.Employees.Where(e => e.IsActive);

            // Tür verilmişse yalnız o tür; verilmemişse bütün tesisler 
            if (branchType.HasValue)
            {
                branches = branches.Where(b => b.BranchType == branchType.Value);
                employees = employees.Where(e => e.Branch.BranchType == branchType.Value);
            }

            // Kayıt yokken SumAsync 0 döner (EF Core boş toplamı 0'a çevirir)
            var totalCapacity = await branches.SumAsync(b => b.Capacity);
            var totalDockCount = await branches.SumAsync(b => b.DockCount);
            var employeeCount = await employees.CountAsync();
            var courierCount = await employees.CountAsync(e => e.JobType == EmployeeJobType.Courier);

            return (employeeCount, courierCount, totalCapacity, totalDockCount);
        }

    }
}
