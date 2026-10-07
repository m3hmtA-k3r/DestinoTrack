using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Cargos
{
    public class CargoRepository(AppDbContext context) : GenericRepository<Cargo>(context), ICargoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Cargo> GetByTrackCodeAsync(string trackCode)
        {
            return await _context.Cargos
                .FirstOrDefaultAsync(c => c.TrackCode == trackCode);
        }

        // Gösterge panelinde durum kırılımı için (dağıtımda / teslim edildi ...)
        public async Task<int> CountByStatusAsync(CargoStatus status)
        {
            return await _context.Cargos.CountAsync(c => c.CargoStatus == status);
        }

        public async Task<bool> TrackCodeExistsAsync(string trackCode)
        {
            // IgnoreQueryFilters: silinmiş bir kargonun numarası yeniden verilmemeli,
            // yoksa eski takip bağlantısı başka bir gönderiyi gösterir
            return await _context.Cargos
                .IgnoreQueryFilters()
                .AnyAsync(c => c.TrackCode == trackCode);
        }

    }
}
