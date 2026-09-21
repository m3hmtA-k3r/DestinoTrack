using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.BranchDtos
{
    // Şube listesinin üstündeki tür çipleri: "Şube 12 · Transfer Merkezi 3 · Ana Depo 1 · Lojistik Park 0"
    public class BranchTypeFilterDto
    {
        public BranchType BranchType { get; set; }
        public int BranchCount { get; set; }
    }
}
