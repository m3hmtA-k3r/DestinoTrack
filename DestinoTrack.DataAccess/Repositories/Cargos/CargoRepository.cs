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

        public async Task<Cargo?> GetByTrackCodeAsync(string trackCode)
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

        public async Task<(List<Cargo> Items, int TotalCount)> GetPagedScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                           Guid? userId, bool unrestricted,
                                                                           CargoStatus? status, string? search, int page, int pageSize)
        {
            IQueryable<Cargo> query = Scoped(branchId, countryId, customerId, userId, unrestricted)
                        .Include(c => c.OriginBranch)
                        .Include(c => c.DestinationBranch);


            if (status.HasValue)
            {
                query = query.Where(c => c.CargoStatus == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.TrackCode.Contains(term) || c.SenderName.Contains(term) || c.ReceiverName.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.ShipmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Cargo?> GetScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, unrestricted)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // kapsamın sorgu karşılığı — iki metot da aynı kuralı kullanır, kural tek yerde durur
        private IQueryable<Cargo> Scoped(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId, bool unrestricted)
        {
            IQueryable<Cargo> query = _context.Cargos;

            if (unrestricted)
            {
                return query;   // Admin
            }

            if (branchId.HasValue)
            {
                // Personel / Kurye: şubesinden çıkan ya da şubesine gelen
                return query.Where(c => c.OriginBranchId == branchId.Value || c.DestinationBranchId == branchId.Value);
            }

            if (countryId.HasValue)
            {
                // Manager: ülkesindeki şubeler üzerinden geçen
                return query.Where(c => c.OriginBranch.City.CountryId == countryId.Value
                                        || c.DestinationBranch.City.CountryId == countryId.Value);
            }

            if (customerId.HasValue)
            {
                return query.Where(c => c.CustomerId == customerId.Value);
            }

            if (userId.HasValue)
            {
                return query.Where(c => c.SenderId == userId.Value || c.ReceiverId == userId.Value);
            }

            // Kapsamı belirsiz kullanıcı hiçbir şey görmez
            return query.Where(c => false);
        }


        public async Task<Cargo?> GetDetailScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, unrestricted)
                .Include(c => c.OriginBranch)
                .Include(c => c.DestinationBranch)
                .Include(c => c.Courier)
                .Include(c => c.Customer)
                .Include(c => c.Delivery)
                    .ThenInclude(d => d.DeliveredByEmployee)
                .Include(c => c.DeliveryExceptions)
                .Include(c => c.Movements)
                    .ThenInclude(m => m.Branch)
                .Include(c => c.Movements)
                    .ThenInclude(m => m.PerformedByUser)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<Cargo?> GetLabelScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId,
                                              Guid? userId, bool unrestricted)
        {
            // Etikette yalnız şube adı değil, şube + şehir + varış ülkesi yazar — zincirli Include bu yüzden.
            return await Scoped(branchId, countryId, customerId, userId, unrestricted)
                .Include(c => c.OriginBranch)
                    .ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch)
                    .ThenInclude(b => b.City)
                        .ThenInclude(ct => ct.Country)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cargo?> GetPublicByTrackCodeAsync(string trackCode)
        {
            // Scoped(...) YOK — gerekçe arayüzde. Silinmiş kargo global süzgeçle zaten dışarıda.
            // Çizelgede "şube adı, şehir" yazacağı için hareketin şubesi ve şehri de çekilir  
            return await _context.Cargos
                .Include(c => c.OriginBranch)
                    .ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch)
                    .ThenInclude(b => b.City)
                .Include(c => c.Movements)
                    .ThenInclude(m => m.Branch)
                        .ThenInclude(b => b.City)
                .FirstOrDefaultAsync(c => c.TrackCode == trackCode);
        }



    } 
}
