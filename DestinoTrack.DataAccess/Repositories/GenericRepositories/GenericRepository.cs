using DestinoTrack.DataAccess.Context;
using DestinoTrack.Entity.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.GenericRepositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        // Gösterge panelindeki sayaçlar için. Tüm kayıtları belleğe çekmek
        public async Task<int> CountAsync()
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        // BaseEntityInterceptor bu silmeyi soft delete'e çevirir 
        public async Task DeleteAsync(TEntity entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // ExecuteDelete SaveChanges'tan geçmez → interceptor çalışmaz, satır gerçekten silinir 
        public async Task HardDeleteAsync(TEntity entity)
        {
            await _context.Set<TEntity>().IgnoreQueryFilters().Where(e => e.Id == entity.Id).ExecuteDeleteAsync();
            _context.Entry(entity).State = EntityState.Detached;
        }

    }
}
