using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DataAccess.Repositories.AuditLogs
{
    // Jenerik IRepository'den BİLEREK türemiyor: onun `where TEntity : BaseEntity` kısıtı var ve
    // AuditLog ondan türemiyor — denetim kaydı güncellenmez, silinmez, kendisi denetlenmez.
    // Burada YALNIZ okuma var; Add/Update/Delete hiç tanımlanmadı ki ekrandan yazılamasın
    public interface IAuditLogRepository
    {
        Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(AuditAction? action, string? entityName,
                                                                   string? search, int page, int pageSize);

        // Süzme çipleri için: kayıtta GERÇEKTEN geçen tür adları (sabit liste değil)
        Task<List<string>> GetEntityNamesAsync();
    }
}
