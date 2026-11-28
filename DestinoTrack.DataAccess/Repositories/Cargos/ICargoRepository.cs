using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.Cargos
{
    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<Cargo?> GetByTrackCodeAsync(string trackCode);

        // üretilen takip no daha önce kullanılmış mı (silinmişler dahil — numara geri dönüşmez)
        Task<bool> TrackCodeExistsAsync(string trackCode);

        // kapsam sorguda uygulanır — sayfalama ve toplam sayı doğru çıksın.
        // courierEmployeeId doluysa kurye kapsamı: yalnız o kuryeye atanmış kargolar
        Task<(List<Cargo> Items, int TotalCount)> GetPagedScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                       Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                       CargoStatus? status, string? search, int page, int pageSize,
                                                                       CargoDirection? direction = null);

        Task<Cargo?> GetScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                    Guid? courierEmployeeId, bool unrestricted);

    
        Task<Cargo?> GetDetailScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                          Guid? courierEmployeeId, bool unrestricted);

        Task<Cargo?> GetLabelScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                         Guid? courierEmployeeId, bool unrestricted);

        Task<Cargo?> GetByTrackCodeScopedAsync(string trackCode, Guid? branchId, Guid? countryId, Guid? customerId,
                                               Guid? userId, Guid? courierEmployeeId, bool unrestricted);


        Task<(List<Cargo> Items, int TotalCount)> GetDeliveryListScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                             Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                             CargoStatus[] statuses, int page, int pageSize,
                                                                             CargoDirection? direction = null);

        Task<(List<Cargo> Items, int TotalCount)> GetIssuesScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                        Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                        CargoIssue? issue, int page, int pageSize);



        // Günlük özetin anlık kartları — liste değil sayı. Gecikme ve sorun kuralı
        // listeyle AYNI `WithIssue`'dan geçiyor: ekranda 5 sorunlu kargo varsa kart da 5 der
        Task<int> CountIssuesScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                         Guid? courierEmployeeId, bool unrestricted, CargoIssue? issue);

        Task<int> CountByStatusScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                           Guid? courierEmployeeId, bool unrestricted, params CargoStatus[] statuses);
        Task<int> CountScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted);

        Task<Dictionary<CargoStatus, int>> CountGroupedByStatusScopedAsync(Guid? branchId, Guid? countryId,
                                   Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted);
        Task<Dictionary<DateTime, int>> CountByShipmentDayScopedAsync(Guid? branchId, Guid? countryId,
                                   Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                   DateTime from, DateTime to);
        Task<List<(Guid BranchId, string BranchName, int Count)>> TopBranchesByTrafficScopedAsync(
                                   Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted, int take);
        
        // Rapor · şubeyi VARIŞ kargosundaki gecikmeye göre sıralar. Trafik kardeşi
        // kargoyu iki uçta da sayar, bu yalnız varışta: gecikme varış taahhüdünün ihlali
        Task<List<(Guid BranchId, string BranchName, int DelayedCount)>> TopBranchesByDelayScopedAsync(
                                   Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted, int take);


        // Rapor · personel başına teslim ve başarısız DENEME sayısı.
        // ⚠️ Buradaki teslim sayısı Kurye & Dağıtım ekranındaki "Bugün Teslim" ile EŞİT DEĞİL:
        // orası yalnız bugünü ve yalnız kuryeleri sayar, burası kapsamın tamamını
        Task<List<(Guid EmployeeId, string EmployeeName, int DeliveredCount, int FailedAttemptCount)>> TopEmployeesByDeliveryScopedAsync(
                                   Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted, int take);

        Task<(int Delivered, int OnTime)> CountOnTimeDeliveredScopedAsync(Guid? branchId, Guid? countryId,
                                   Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted);


        // Rapor · kök neden dağılımları. İkisi de kapsamdan geçiyor: rapor da D47–D50'nin dışında değil.
        // "Yolda neden gecikti" hareketten, "kapıda neden teslim edilemedi" teslim denemesinden —
        // iki ayrı tablo, iki ayrı soru, birleştirilmiyor
        Task<Dictionary<DelayReason, int>> CountDelayReasonsScopedAsync(Guid? branchId, Guid? countryId,
                                   Guid? customerId, Guid? userId, Guid? courierEmployeeId, bool unrestricted);

        Task<Dictionary<DeliveryFailureReason, int>> CountFailureReasonsScopedAsync(Guid? branchId,
                                   Guid? countryId, Guid? customerId, Guid? userId,
                                   Guid? courierEmployeeId, bool unrestricted);
        Task<Cargo?> GetLatestScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                           Guid? courierEmployeeId, bool unrestricted);

        Task<Cargo?> GetPublicByTrackCodeAsync(string trackCode);
    }
}
