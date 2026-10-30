using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.CargoMovements
{
    public interface ICargoMovementRepository: IRepository<CargoMovement>
    {
        // Günlük özet: "bugün gelen / çıkan / teslim edilen" kargonun kendi tarihlerinden değil,
        // bugün BU ŞUBEDE yazılan hareketlerden sayılıyor — gişede bugün ne olduğunun tek dgru kaynagı.
        // Tek sorguda gruplanıp sozluk döner, üç kart için üç tur atılmıyor;
        // o gün hiç yazılmamış bir durum sözlükte hiç bulunmaz
        Task<Dictionary<CargoStatus, int>> CountTodayByNewStatusAsync(Guid branchId);

        // VC: şubede en son yazılan hareketler. Takip numarası için kargo,"kim yaptı" için kullanıcı birlikte gelir
        Task<List<CargoMovement>> GetRecentByBranchAsync(Guid branchId, int count);
    }
}
