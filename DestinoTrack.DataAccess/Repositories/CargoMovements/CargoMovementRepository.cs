using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.CargoMovements
{
    public class CargoMovementRepository(AppDbContext context) : GenericRepository<CargoMovement>(context), ICargoMovementRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Dictionary<CargoStatus, int>> CountTodayByNewStatusAsync(Guid branchId)
        {
            // Gün sınırı UTC'den: hareket kaydı UtcNow ile yazılıyor, gecikme kuralı da UTC okuyor.
            // Yerel saate çevirmek iki yerde iki farklı "bugün" çıkarırdı
            var bugun = DateTime.UtcNow.Date;
            var yarin = bugun.AddDays(1);

            return await _context.CargoMovements
                .Where(m => m.BranchId == branchId
                            && m.MovementDate >= bugun
                            && m.MovementDate < yarin)
                .GroupBy(m => m.CargoStatus)
                .Select(g => new { Durum = g.Key, Adet = g.Count() })
                .ToDictionaryAsync(x => x.Durum, x => x.Adet);
        }

        public async Task<List<CargoMovement>> GetRecentByBranchAsync(Guid branchId, int count)
        {
            // Kargo ve kullanıcı tekil bağlantı — tek sorgu yeter, AsSplitQuery gerekmiyor
            return await _context.CargoMovements
                .Where(m => m.BranchId == branchId)
                .Include(m => m.Cargo)
                .Include(m => m.PerformedByUser)
                .OrderByDescending(m => m.MovementDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
