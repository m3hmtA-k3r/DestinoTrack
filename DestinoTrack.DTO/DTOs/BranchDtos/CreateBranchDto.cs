using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.BranchDtos
{
    public class CreateBranchDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public BranchType BranchType { get; set; } = BranchType.Branch;

        public Guid CityId { get; set; }

        public int Capacity { get; set; }
        public int DockCount { get; set; }
    }
}
