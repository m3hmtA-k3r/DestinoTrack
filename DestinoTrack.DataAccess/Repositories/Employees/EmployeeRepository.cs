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
            // Vehicle: araç bilgisi artık Employee'de değil, havuzda (D98) — include olmazsa
            // lazy loading sayfadaki her personel için ayrı sorgu açar
            IQueryable<Employee> query = _context.Employees
                .Include(e => e.Branch)
                    .ThenInclude(b => b.City)
                .Include(e => e.Vehicle);

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

        public async Task<bool> HasEmployeeAsync(Guid appUserId)
        {
            return await _context.Employees.AnyAsync(e => e.AppUserId == appUserId);
        }


        public async Task<Employee?> GetByAppUserAsync(Guid appUserId)
        {
            // Giriş yapmayan personelde AppUserId boş; eşleşen tek kayıt beklenir
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.AppUserId == appUserId);
        }

        public async Task<List<Employee>> GetActiveCouriersAsync(Guid branchId)
        {
            // Pasif kurye listede çıkmaz: DispatchAsync onu reddediyor, boşuna seçenek göstermeyelim
            return await _context.Employees
                .Where(e => e.BranchId == branchId && e.JobType == EmployeeJobType.Courier && e.IsActive)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<(List<Employee> Items, int TotalCount)> GetCourierOverviewAsync(Guid? branchId, VehicleType? vehicleType,
                                                                                  bool? isActive, string? search, int page, int pageSize)
        {
            // Yalnız kuryeler. Araç havuzdan geldiği için Vehicle include ediliyor 
            IQueryable<Employee> query = _context.Employees
                .Include(e => e.Branch)
                .Include(e => e.Vehicle)
                .Where(e => e.JobType == EmployeeJobType.Courier);

            if (branchId.HasValue)
            {
                query = query.Where(e => e.BranchId == branchId.Value);
            }

            // Araç türü süzgeci aracın üzerinden: aracı olmayan kurye bu süzgeçte görünmez
            if (vehicleType.HasValue)
            {
                query = query.Where(e => e.Vehicle != null && e.Vehicle.VehicleType == vehicleType.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(e => e.FirstName.Contains(term)
                                      || e.LastName.Contains(term)
                                      || (e.Vehicle != null && e.Vehicle.Plate.Contains(term)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Dictionary<Guid, int>> GetOnDeliveryCountsAsync()
        {
            return await _context.Cargos
                .Where(c => c.CourierId != null && c.CargoStatus == CargoStatus.OutForDelivery)
                .GroupBy(c => c.CourierId!.Value)
                .Select(g => new { CourierId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CourierId, x => x.Count);
        }

        public async Task<Dictionary<Guid, int>> GetDeliveredTodayCountsAsync()
        {
            // Tarihler UTC saklanıyor; "bugün" tanımı CargoMovementRepository ile aynı
            var bugun = DateTime.UtcNow.Date;

            return await _context.Deliveries
                .Where(d => d.DeliveredAt >= bugun)
                .GroupBy(d => d.DeliveredByEmployeeId)
                .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Count);
        }
    }
}
