using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.CargoMovements
{
    public interface ICargoMovementRepository: IRepository<CargoMovement>
    {
        Task<Dictionary<CargoStatus, int>> CountTodayByNewStatusAsync(Guid branchId);
        Task<List<CargoMovement>> GetRecentByBranchAsync(Guid branchId, int count);


        // Ülke kapsamı (Manager paneli): hareketin yazıldığı şube bu ülkede mi.
        // Transfer merkezleri de dahil — onlar da şube türünde ve ülkenin hareketini taşıyor
        Task<Dictionary<CargoStatus, int>> CountTodayByCountryAsync(Guid countryId);

        // Ülkedeki en son hareketler. Şube sürümünden farkı: satırda HANGİ ŞUBE
        // olduğu da gerekiyor, Manager birden çok şubeye bakabiir
        Task<List<CargoMovement>> GetRecentByCountryAsync(Guid countryId, int count);

    }
}
