using System.Text.Encodings.Web;
using System.Text.Json;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DestinoTrack.DataAccess.Interceptors
{
    // Her SaveChanges'ta BaseEntity değişikliklerini AuditLog'a yazar  
    // BaseEntityInterceptor'dan SONRA çalışır: soft delete burada "IsDeleted false => "true" güncellemesi olarak görünür
    public class AuditLogInterceptor(ICurrentUserAccessor _currentUser) : SaveChangesInterceptor
    {
        // Türkçe karakterler JSON'da okunur kalsın ("Şehir" — "\u015Eehir" değil)
        private static readonly JsonSerializerOptions JsonOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        private static readonly HashSet<string> Ignored = [nameof(BaseEntity.UpdatedDate)];

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context != null)
            {
                var entries = Changed(context);
                var databaseValues = new Dictionary<EntityEntry<BaseEntity>, PropertyValues?>();
                foreach (var entry in entries.Where(e => e.State == EntityState.Modified))
                {
                    databaseValues[entry] = entry.GetDatabaseValues();
                }
                AddLogs(context, entries, databaseValues);
            }
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context != null)
            {
                var entries = Changed(context);
                var databaseValues = new Dictionary<EntityEntry<BaseEntity>, PropertyValues?>();
                foreach (var entry in entries.Where(e => e.State == EntityState.Modified))
                {
                    databaseValues[entry] = await entry.GetDatabaseValuesAsync(cancellationToken);
                }
                AddLogs(context, entries, databaseValues);
            }
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        // Denetlenecekler: eklenen ve güncellenen BaseEntity kayıtları (--> silme de buraya güncelleme olarak gelir)
        private static List<EntityEntry<BaseEntity>> Changed(DbContext context)
        {
            context.ChangeTracker.DetectChanges();
            return context.ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified)
                .ToList();
        }

        private void AddLogs(DbContext context, List<EntityEntry<BaseEntity>> entries, Dictionary<EntityEntry<BaseEntity>, PropertyValues?> databaseValues)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                AuditAction action;
                Dictionary<string, object?>? oldValues = null;
                Dictionary<string, object?>? newValues;

                if (entry.State == EntityState.Added)
                {
                    action = AuditAction.Create;
                    newValues = Snapshot(entry, null);
                }
                else
                {
                    var database = databaseValues.GetValueOrDefault(entry);
                    var wasDeleted = database != null && (bool)database[nameof(BaseEntity.IsDeleted)]!;

                    if (entry.Entity.IsDeleted && !wasDeleted)
                    {
                        // Soft delete: silinen kaydın son hali eski değer olarak saklanır
                        action = AuditAction.Delete;
                        oldValues = Snapshot(entry, database);
                        newValues = new() { [nameof(BaseEntity.IsDeleted)] = true };
                    }
                    else
                    {
                        action = AuditAction.Update;
                        oldValues = new();
                        newValues = new();
                        foreach (var property in entry.Properties.Where(p => p.IsModified && !Ignored.Contains(p.Metadata.Name)))
                        {
                            // Eski değer veritabanından: servis kaydı DTO'dan kurmuş olsa da gerçek önceki hal
                            var before = database != null ? database[property.Metadata] : property.OriginalValue;
                            if (!Equals(before, property.CurrentValue))
                            {
                                oldValues[property.Metadata.Name] = before;
                                newValues[property.Metadata.Name] = property.CurrentValue;
                            }
                        }

                        // Formu hiçbir şey değiştirmeden kaydetmek: değişen alan yok → log da yok
                        if (newValues.Count == 0)
                        {
                            continue;
                        }
                    }
                }

                context.Add(new AuditLog
                {
                    UserId = _currentUser.UserId,
                    UserEmail = _currentUser.Email,
                    Action = action,
                    EntityName = entry.Metadata.ClrType.Name,
                    EntityId = entry.Entity.Id,
                    CreatedDate = now,
                    OldValues = oldValues == null ? null : JsonSerializer.Serialize(oldValues, JsonOptions),
                    NewValues = JsonSerializer.Serialize(newValues, JsonOptions)
                });
            }
        }

        // Kaydın bütün alanları: values verilirse oradan (veritabanındaki hal), verilmezse şu anki değerler
        private static Dictionary<string, object?> Snapshot(EntityEntry<BaseEntity> entry, PropertyValues? values) =>
            entry.Properties
                .Where(p => !Ignored.Contains(p.Metadata.Name))
                .ToDictionary(p => p.Metadata.Name, p => values != null ? values[p.Metadata] : p.CurrentValue);
    }
}
