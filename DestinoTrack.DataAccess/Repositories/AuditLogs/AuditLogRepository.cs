using DestinoTrack.DataAccess.Context;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DestinoTrack.DataAccess.Repositories.AuditLogs
{
    public class AuditLogRepository(AppDbContext context) : IAuditLogRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(AuditAction? action,
                                   string? entityName, string? search, int page, int pageSize)
        {
            IQueryable<AuditLog> query = _context.AuditLogs;

            if (action is not null)
            {
                query = query.Where(a => a.Action == action);
            }

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(a => a.EntityName == entityName);
            }

            // Arama kullanıcı e-postasında: "bu işlemi kim yaptı" denetimin ilk sorusu
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.UserEmail != null && a.UserEmail.Contains(search));
            }

            var toplam = await query.CountAsync();

            // En yeniden eskiye: denetimde önce "en son ne oldu" sorulur
            var kayitlar = await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (kayitlar, toplam);
        }

        public async Task<List<string>> GetEntityNamesAsync()
        {
            return await _context.AuditLogs
                .Select(a => a.EntityName)
                .Distinct()
                .OrderBy(ad => ad)
                .ToListAsync();
        }
    }
}
