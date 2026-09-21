using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.BranchDtos
{
    public class ResultBranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public BranchType BranchType { get; set; }

        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }

        public int Capacity { get; set; }
        public int DockCount { get; set; }

        // CurrentLoad bilerek yok 
    }
}
