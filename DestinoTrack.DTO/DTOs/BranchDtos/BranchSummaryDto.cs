using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.BranchDtos
{
    // BranchSummary kartı: liste ekranındaki tür çiplerini tekrar etmeyen tamamlayıcı sayılar
    public class BranchSummaryDto
    {
        public BranchType? BranchType { get; set; }

        // Kapsamdaki tesislerde çalışan aktif personel (pasifler sayılmaz) ve bunların kaçının kurye olduğu
        public int EmployeeCount { get; set; }
        public int CourierCount { get; set; }

        public int TotalCapacity { get; set; }
        public int TotalDockCount { get; set; }
    }
}
