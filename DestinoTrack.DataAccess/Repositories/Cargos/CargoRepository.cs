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

        public async Task<bool> TrackCodeExistsAsync(string trackCode)
        {
            // IgnoreQueryFilters: silinmiş bir kargonun numarası yeniden verilmemeli,
            return await _context.Cargos
                .IgnoreQueryFilters()
                .AnyAsync(c => c.TrackCode == trackCode);
        }

        public async Task<(List<Cargo> Items, int TotalCount)> GetPagedScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                   Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                   CargoStatus? status, string? search, int page, int pageSize,
                                                                   CargoDirection? direction = null)
        {
            IQueryable<Cargo> query = Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted, direction)
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

        public async Task<Cargo?> GetScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                                 Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<Cargo?> GetByTrackCodeScopedAsync(string trackCode, Guid? branchId, Guid? countryId,
                                                            Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .Include(c => c.OriginBranch)
                .Include(c => c.DestinationBranch)
                .Include(c => c.Movements)
                .FirstOrDefaultAsync(c => c.TrackCode == trackCode);
        }


        public async Task<(List<Cargo> Items, int TotalCount)> GetDeliveryListScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                      Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                      CargoStatus[] statuses, int page, int pageSize,
                                                                      CargoDirection? direction = null)
        {
            var query = Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted, direction)
                .Where(c => statuses.Contains(c.CargoStatus))
                .Include(c => c.DestinationBranch)
                .Include(c => c.Courier)
                .Include(c => c.DeliveryExceptions);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.EstimatedArrivalDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            return (items, totalCount);
        }


        public async Task<(List<Cargo> Items, int TotalCount)> GetIssuesScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                        Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                        CargoIssue? issue, int page, int pageSize)
        {
            var query = WithIssue(Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted), issue)
                .Include(c => c.DestinationBranch)
                .Include(c => c.Courier)
                .Include(c => c.DeliveryExceptions);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.EstimatedArrivalDate)   // en çok geciken üstte
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            return (items, totalCount);
        }



        public async Task<int> CountIssuesScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                                      Guid? courierEmployeeId, bool unrestricted, CargoIssue? issue)
        {
            // Aynı kural, aynı metot — yalnız sonu farklı: liste yerine sayı
            return await WithIssue(Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted), issue)
                .CountAsync();
        }


        public async Task<int> CountByStatusScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                                 Guid? courierEmployeeId, bool unrestricted, params CargoStatus[] statuses)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .CountAsync(c => statuses.Contains(c.CargoStatus));
        }


        public async Task<int> CountScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                                Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .CountAsync();
        }


        public async Task<Dictionary<CargoStatus, int>> CountGroupedByStatusScopedAsync(Guid? branchId, Guid? countryId,
                                   Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .GroupBy(c => c.CargoStatus)
                .Select(g => new { Durum = g.Key, Adet = g.Count() })
                .ToDictionaryAsync(x => x.Durum, x => x.Adet);
        }


        public async Task<Dictionary<DateTime, int>> CountByShipmentDayScopedAsync(Guid? branchId, Guid? countryId,
                                   Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                   DateTime from, DateTime to)
        {
            // Gruplama `ShipmentDate.Date` ile: SQL tarafında CAST(... AS date) oluyor,
            // yani saat farkı günleri bölmüyor
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .Where(c => c.ShipmentDate >= from && c.ShipmentDate < to)
                .GroupBy(c => c.ShipmentDate.Date)
                .Select(g => new { Gun = g.Key, Adet = g.Count() })
                .ToDictionaryAsync(x => x.Gun, x => x.Adet);
        }


        public async Task<List<(Guid BranchId, string BranchName, int Count)>> TopBranchesByTrafficScopedAsync(
                                   Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted, int take)
        {
            var kapsam = Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted);

            var dokunan = kapsam.Select(c => new { Id = c.OriginBranchId, Ad = c.OriginBranch.Name })
                .Concat(kapsam.Select(c => new { Id = c.DestinationBranchId, Ad = c.DestinationBranch.Name }));

            var liste = await dokunan
                .GroupBy(x => new { x.Id, x.Ad })
                .Select(g => new { g.Key.Id, g.Key.Ad, Adet = g.Count() })
                // Sıra BELİRLİ olmak zorunda: eşit adette şube adı ayırır.
                // Yoksa iki şubenin sırası sorgudan sorguya değişir ve derleme yine yeşil geçer
                .OrderByDescending(x => x.Adet).ThenBy(x => x.Ad)
                .Take(take)
                .ToListAsync();

            return liste.Select(x => (x.Id, x.Ad, x.Adet)).ToList();
        }

        public async Task<(int Delivered, int OnTime)> CountOnTimeDeliveredScopedAsync(Guid? branchId,
                                   Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted)
        {
            var teslim = Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .Where(c => c.CargoStatus == CargoStatus.Delivered);

            var toplam = await teslim.CountAsync();

            // Durum makinesi gereği kargo başına TEK teslim hareketi var (Delivered'ın çıkışı yok),
            // bu yüzden `Any` yeterli: o tek hareketin tarihi tahmini varışta ya da öncesinde mi
            var zamaninda = await teslim.CountAsync(c => c.Movements
                .Any(m => m.CargoStatus == CargoStatus.Delivered
                          && m.MovementDate <= c.EstimatedArrivalDate));

            return (toplam, zamaninda);
        }


        public async Task<Dictionary<DelayReason, int>> CountDelayReasonsScopedAsync(Guid? branchId,
                                   Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted)
        {
            // Kapsam kargo tarafında tanımlı; hareketler oradan süzülüyor (SQL'de IN alt sorgusu)
            var kapsamIds = Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .Select(c => c.Id);

            return await _context.CargoMovements
                .Where(m => m.DelayReason != null && kapsamIds.Contains(m.CargoId))
                .GroupBy(m => m.DelayReason!.Value)
                .Select(g => new { Neden = g.Key, Adet = g.Count() })
                .ToDictionaryAsync(x => x.Neden, x => x.Adet);
        }

        public async Task<Dictionary<DeliveryFailureReason, int>> CountFailureReasonsScopedAsync(Guid? branchId,
                                   Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted)
        {
            var kapsamIds = Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .Select(c => c.Id);

            return await _context.DeliveryExceptions
                .Where(e => kapsamIds.Contains(e.CargoId))
                .GroupBy(e => e.Reason)
                .Select(g => new { Neden = g.Key, Adet = g.Count() })
                .ToDictionaryAsync(x => x.Neden, x => x.Adet);
        }

        public async Task<Cargo?> GetLatestScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                                Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
                .Include(c => c.Movements).ThenInclude(m => m.Branch).ThenInclude(b => b.City)
                .OrderByDescending(c => c.Movements.Max(m => (DateTime?)m.MovementDate) ?? c.CreatedDate)
                .ThenByDescending(c => c.CreatedDate)
                .FirstOrDefaultAsync();
        }


        //iki kardeşi BURADA tanımlı. Başka hiçbir yerde "geciken" hesabı yazılmayacak:
        // sorunlu kargolar listesi de, günlük özetteki sayı da bu metottan geçer
        private static IQueryable<Cargo> WithIssue(IQueryable<Cargo> query, CargoIssue? issue)
        {
            var simdi = DateTime.UtcNow;

            query = query.Where(c => c.CargoStatus != CargoStatus.Delivered
                                     && c.CargoStatus != CargoStatus.ReturnedToSender);

            return issue switch
            {
                CargoIssue.FailedAttempt => query.Where(c => c.DeliveryExceptions.Any()),

                CargoIssue.ReturnInProgress => query.Where(c => c.CargoStatus == CargoStatus.ReturnInProgress),

                CargoIssue.Delayed => query.Where(c => c.EstimatedArrivalDate < simdi),

                _ => query.Where(c => c.DeliveryExceptions.Any()
                                      || c.CargoStatus == CargoStatus.ReturnInProgress
                                      || c.EstimatedArrivalDate < simdi)
            };
        }


        private IQueryable<Cargo> Scoped(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                         Guid? courierEmployeeId, bool unrestricted,
                                         CargoDirection? direction = null)
        {
            IQueryable<Cargo> query = _context.Cargos;

            if (unrestricted)
            {
                return query;
            }

            if (courierEmployeeId.HasValue)
            {
                // Kurye şubesinin tamamını değil, yalnız kendisine atanmış kargoları görür ().
                // Önce bakılıyor: kuryenin şubesi de olsa kapsamı bu daha dar kural belirler
                return query.Where(c => c.CourierId == courierEmployeeId.Value);
            }

            if (branchId.HasValue)
            {
                // Yön verilmişse liste tek tarafa daralır — şube panelindeki "Gelen / Çıkan" çipleri
                return direction switch
                {
                    CargoDirection.Incoming => query.Where(c => c.DestinationBranchId == branchId.Value),
                    CargoDirection.Outgoing => query.Where(c => c.OriginBranchId == branchId.Value),
                    _ => query.Where(c => c.OriginBranchId == branchId.Value || c.DestinationBranchId == branchId.Value)
                };
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
                // Bireysel
                return direction switch
                {
                    CargoDirection.Incoming => query.Where(c => c.ReceiverId == userId.Value),
                    CargoDirection.Outgoing => query.Where(c => c.SenderId == userId.Value),
                    _ => query.Where(c => c.SenderId == userId.Value || c.ReceiverId == userId.Value)
                };
            }


            return query.Where(c => false);
        }

        public async Task<Cargo?> GetDetailScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                                       Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
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
                                              Guid? userId, Guid? courierEmployeeId, bool unrestricted)
        {
            return await Scoped(branchId, countryId, customerId, userId, courierEmployeeId, unrestricted)
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
            //YOK — gerekçe arayüzde. Silinmiş kargo global süzgeçle zaten dışarıda.
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
