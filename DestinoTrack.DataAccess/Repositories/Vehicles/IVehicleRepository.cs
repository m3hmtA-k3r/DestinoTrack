using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.Vehicles
{
    // Vehicle bir BaseEntity, bu yüzden jenerik depodan türüyor (AuditLog'un tersi: o türemiyordu)
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        // Sayfalı liste: şube · araç türü · durum süzgeçleri + plakada arama
        Task<(List<Vehicle> Items, int TotalCount)> GetPagedAsync(Guid? branchId, VehicleType? vehicleType,
                                                                  bool? isActive, string? search, int page, int pageSize);

        // Kurye formundaki açılır liste: yalnızca o şubenin AKTİF araçları
        Task<List<Vehicle>> GetLookupAsync(Guid? branchId = null);

        // Plaka tekilliği: mesajı kullanıcıya bu sorgu yazdırır, indeks son savunma
        Task<bool> PlateExistsAsync(string plate, Guid? excludeId);

        // Silme öncesi: araca bağlı kurye var mı 
        Task<bool> HasCouriersAsync(Guid vehicleId);
        //sunucu tarafı: cascade script yalnız gizler, kuralı bu uygular
        Task<bool> BelongsToBranchAsync(Guid vehicleId, Guid branchId);

        // VehicleId → o araçtaki kurye sayısı. Tek sorgu, kayıtlar belleğe çekilmez
        Task<Dictionary<Guid, int>> GetCourierCountsAsync();
    }
}
