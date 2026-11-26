using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.VehicleDtos
{
    public class UpdateVehicleDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; }
        public VehicleType? VehicleType { get; set; }
        public Guid BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}
