using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.BranchDtos
{
    public class UpdateBranchDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public BranchType BranchType { get; set; }

        public Guid CityId { get; set; }

        public int Capacity { get; set; }
        public int DockCount { get; set; }
    }
}
