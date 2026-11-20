using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.Cargos
{
    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<Cargo?> GetByTrackCodeAsync(string trackCode);
        Task<int> CountByStatusAsync(CargoStatus status);

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
        Task<Cargo?> GetLatestScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                           Guid? courierEmployeeId, bool unrestricted);

        Task<Cargo?> GetPublicByTrackCodeAsync(string trackCode);
    }
}
