using DestinoTrack.Business.Consts;
using DestinoTrack.DataAccess.Repositories.AuditLogs;
using DestinoTrack.DTO.DTOs.AuditDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.AuditLogs
{
    public class AuditLogService(IAuditLogRepository _auditLogRepository) : IAuditLogService
    {
        public async Task<PagedResult<AuditLogRowDto>> GetPagedAsync(AuditAction? action = null,
                                   string? entityName = null, string? search = null, int page = 1)
        {
            // Adres çubuğuna ?page=0 yazılabilir: en küçük sayfa 1
            page = page < 1 ? 1 : page;

            var (kayitlar, toplam) = await _auditLogRepository.GetPagedAsync(action, entityName, search, page, Paging.PageSize);

            // Süzgeç daraldığında eldeki sayfa numarası aralığın dışında kalabilir → son sayfaya düş
            if (kayitlar.Count == 0 && toplam > 0)
            {
                page = (int)Math.Ceiling(toplam / (double)Paging.PageSize);
                (kayitlar, toplam) = await _auditLogRepository.GetPagedAsync(action, entityName, search, page, Paging.PageSize);
            }

            return new PagedResult<AuditLogRowDto>
            {
                Items = kayitlar.Select(a => new AuditLogRowDto
                {
                    CreatedDate = a.CreatedDate,
                    UserEmail = a.UserEmail,
                    Action = a.Action,
                    EntityName = a.EntityName,
                    EntityId = a.EntityId,
                    OldValues = a.OldValues,
                    NewValues = a.NewValues
                }).ToList(),

                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = toplam
            };
        }

        public async Task<List<string>> GetEntityFiltersAsync()
        {
            return await _auditLogRepository.GetEntityNamesAsync();
        }
    }
}
