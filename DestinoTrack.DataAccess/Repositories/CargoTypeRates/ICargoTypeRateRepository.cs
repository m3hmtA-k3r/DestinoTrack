using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.CargoTypeRates
{
    public interface ICargoTypeRateRepository : IRepository<CargoTypeRate>
    {
        // Fiyat hesabı: kargo tipinin çarpanı ve gün farkı (yoksa null)
        Task<CargoTypeRate?> GetByTypeAsync(CargoType cargoType);

        // Admin ekranı: sekiz tip, enum sırasıyla
        Task<List<CargoTypeRate>> GetAllOrderedAsync();
    }
}
