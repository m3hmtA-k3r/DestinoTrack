using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Deliveries
{
    public interface IDeliveryRepository : IRepository<Delivery>
    {
        // Kargo başına tek satır: varsa kargo zaten teslim edilmiş
        Task<Delivery?> GetByCargoAsync(Guid cargoId);
    }
}
