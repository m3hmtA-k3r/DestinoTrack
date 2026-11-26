using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Vehicles
{
    public class VehicleRepository(AppDbContext context) : GenericRepository<Vehicle>(context), IVehicleRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<(List<Vehicle> Items, int TotalCount)> GetPagedAsync(Guid? branchId, VehicleType? vehicleType,
                                                                              bool? isActive, string? search, int page, int pageSize)
        {
            IQueryable<Vehicle> query = _context.Vehicles.Include(v => v.Branch);

            if (branchId.HasValue)
            {
                query = query.Where(v => v.BranchId == branchId.Value);
            }

            if (vehicleType.HasValue)
            {
                query = query.Where(v => v.VehicleType == vehicleType.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(v => v.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(v => v.Plate.Contains(term));
            }

            // Sayfa sayısı süzgece uyan TÜM kayıtlara göre hesaplanır → önce sayım
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(v => v.Plate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Vehicle>> GetLookupAsync(Guid? branchId = null)
        {
            IQueryable<Vehicle> query = _context.Vehicles.Where(v => v.IsActive);

            if (branchId.HasValue)
            {
                query = query.Where(v => v.BranchId == branchId.Value);
            }

            return await query.OrderBy(v => v.Plate).ToListAsync();
        }

        public async Task<bool> PlateExistsAsync(string plate, Guid? excludeId)
        {
            // Plaka servis tarafında büyük harfe çevrilmiş geliyor; karşılaştırma olduğu gibi yapılır
            return await _context.Vehicles
                .AnyAsync(v => v.Plate == plate && (!excludeId.HasValue || v.Id != excludeId.Value));
        }

        // Soft delete'te veritabanının FK koruması çalışmaz — kural serviste bu sorguyla işler
        public async Task<bool> HasCouriersAsync(Guid vehicleId)
        {
            return await _context.Employees.AnyAsync(e => e.VehicleId == vehicleId);
        }

        public async Task<bool> BelongsToBranchAsync(Guid vehicleId, Guid branchId)
        {
            return await _context.Vehicles.AnyAsync(v => v.Id == vehicleId && v.BranchId == branchId);
        }

        public async Task<Dictionary<Guid, int>> GetCourierCountsAsync()
        {
            return await _context.Employees
                .Where(e => e.VehicleId != null)
                .GroupBy(e => e.VehicleId!.Value)
                .Select(g => new { VehicleId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.VehicleId, x => x.Count);
        }
    }
}
