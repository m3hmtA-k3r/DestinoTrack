using DestinoTrack.Entity.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DestinoTrack.DataAccess.Interceptors
{
    // Her SaveChanges'ta BaseEntity kayıtlarını düzenler  
    // eklemede CreatedDate · güncellemede UpdatedDate · silmede soft delete  
    public class BaseEntityInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Apply(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Apply(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void Apply(DbContext? context)
        {
            if (context == null)
            {
                return;
            }

            // Interceptor, EF'in kendi değişiklik taramasından önce çalışır  
            context.ChangeTracker.DetectChanges();
            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = now;
                        entry.Entity.UpdatedDate = null;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        // Servisler güncellemede kaydı DTO'dan kurabilir: oluşturma tarihi ve silinmiş işareti ezilmez
                        entry.Entity.UpdatedDate = now;
                        entry.Property(e => e.CreatedDate).IsModified = false;
                        entry.Property(e => e.IsDeleted).IsModified = false;
                        break;

                    case EntityState.Deleted:
                        // Gerçek silme yerine işaretle; kayıt geçmişte kalır  
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedDate = now;
                        entry.Property(e => e.CreatedDate).IsModified = false;
                        break;
                }
            }
        }
    }
}
