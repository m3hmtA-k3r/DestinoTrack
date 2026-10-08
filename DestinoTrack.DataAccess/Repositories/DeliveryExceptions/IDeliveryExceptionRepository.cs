using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.DeliveryExceptions
{
    public interface IDeliveryExceptionRepository : IRepository<DeliveryException>
    {
        //  kaçıncı denemedeyiz — sınıra ulaşınca iade süreci başlar
        Task<int> CountByCargoAsync(Guid cargoId);
    }
}
