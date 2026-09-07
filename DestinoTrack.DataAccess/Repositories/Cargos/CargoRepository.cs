using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.Cargos
{
    public class CargoRepository(AppDbContext context) : GenericRepository<Cargo>(context), ICargoRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<Cargo> GetByTrackCodeAsync(string trackCode)
        {
            return await _context.Cargos.FirstOrDefaultAsync(d => d.TrackCode == trackCode); //Müşteri takip kodu ile arama yapıcak ve bulduğu kargoyu döndürecek
        }
    }
}
