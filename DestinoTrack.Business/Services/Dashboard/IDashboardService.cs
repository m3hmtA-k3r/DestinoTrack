using DestinoTrack.DTO.DTOs.DashboardDtos;

namespace DestinoTrack.Business.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
