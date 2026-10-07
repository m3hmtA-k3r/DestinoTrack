using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.CargoPrices
{
    public interface ICargoPriceRepository : IRepository<CargoPrice>
    {
        // Fiyat hesabı: çıkış ülkesinin o kademedeki tarifesi  
        Task<CargoPrice?> GetByCountryAndScopeAsync(Guid countryId, RouteScope routeScope);

        // Admin ekranı: ülkesiyle birlikte, ülke adı ve kademeye göre sıralı
        Task<List<CargoPrice>> GetAllWithCountryAsync();
    }
}
