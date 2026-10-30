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
        // courierEmployeeId doluysa kurye kapsamı: yalnız o kuryeye atanmış kargolar (D73)
        Task<(List<Cargo> Items, int TotalCount)> GetPagedScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                       Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                       CargoStatus? status, string? search, int page, int pageSize,
                                                                       CargoDirection? direction = null);

        // Tek kargo, kapsam içindeyse — kapsam dışındaysa null
        Task<Cargo?> GetScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                    Guid? courierEmployeeId, bool unrestricted);

        // detay sayfası — hareketler, teslimat, başarısız denemeler ve şubelerle birlikte  
        Task<Cargo?> GetDetailScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                          Guid? courierEmployeeId, bool unrestricted);

        // etiket — şube + şehir + ülke ile birlikte; hareketler gerekmez 
        Task<Cargo?> GetLabelScopedAsync(Guid id, Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                         Guid? courierEmployeeId, bool unrestricted);

        // Numarayla ara, ama kapsam içinde: şubesine değmeyen kargoya hareket yazılamaz
        // (girişsiz takip ucu kapsam uygulamaz, o ayrı bir iş)
        Task<Cargo?> GetByTrackCodeScopedAsync(string trackCode, Guid? branchId, Guid? countryId, Guid? customerId,
                                               Guid? userId, Guid? courierEmployeeId, bool unrestricted);


        // Dağıtım ekranı: satırda kurye adı ve deneme sayısı da gerekiyor, o yüzden genel liste
        // sorgusundan ayrı duruyor — oradaki birleştirmeleri bu ekran için şişirmiyoruz.
        // Teslime en yakın tarih üstte: dağıtım turu buna göre kuruluyor
        Task<(List<Cargo> Items, int TotalCount)> GetDeliveryListScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                             Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                             CargoStatus status, int page, int pageSize,
                                                                             CargoDirection? direction = null);


        // Sorunlu kargolar: başarısız denemeli · iade sürecinde · geciken (D66).
        // Gecikme kuralı uygulamada tek bir yerde (`WithIssue`) duruyor — TALP-80'in
        // "geciken" sayısı da aynı yoldan geçecek, iki farklı sayı çıkmasın
        Task<(List<Cargo> Items, int TotalCount)> GetIssuesScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId,
                                                                        Guid? userId, Guid? courierEmployeeId, bool unrestricted,
                                                                        CargoIssue? issue, int page, int pageSize);



        // Günlük özetin anlık kartları — liste değil sayı. Gecikme ve sorun kuralı
        // listeyle AYNI `WithIssue`'dan geçiyor: ekranda 5 sorunlu kargo varsa kart da 5 der
        Task<int> CountIssuesScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                         Guid? courierEmployeeId, bool unrestricted, CargoIssue? issue);

        // "Dağıtımda" kartı. Genel CountByStatusAsync kapsam uygulamıyor — şube paneli
        // kendi kargolarını sayacak, komşu şubenin turunu değil
        Task<int> CountByStatusScopedAsync(Guid? branchId, Guid? countryId, Guid? customerId, Guid? userId,
                                           Guid? courierEmployeeId, bool unrestricted, CargoStatus status);


        // korumayı tahmin edilemez takip numarası sağlar. Çizelge için hareketler de gelir
        Task<Cargo?> GetPublicByTrackCodeAsync(string trackCode);
    }
}
