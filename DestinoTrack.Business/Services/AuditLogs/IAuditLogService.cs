using DestinoTrack.DTO.DTOs.AuditDtos;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.AuditLogs
{
    public interface IAuditLogService
    {
        Task<PagedResult<AuditLogRowDto>> GetPagedAsync(AuditAction? action = null, string? entityName = null,
                                                        string? search = null, int page = 1);

        Task<List<string>> GetEntityFiltersAsync();
    }
}
