using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Cargos
{
    public interface ICargoRepository: IRepository<Cargo>
    {
        Task<Cargo> GetByTrackCodeAsync(string trackCode);
    }
}
