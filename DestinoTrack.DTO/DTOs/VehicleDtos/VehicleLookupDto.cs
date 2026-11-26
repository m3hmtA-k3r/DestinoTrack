using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.VehicleDtos
{
    // Kurye formundaki araç açılır listesi: kurye yalnızca kendi şubesinin araçlarını görür 
    public class VehicleLookupDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; }
        public VehicleType VehicleType { get; set; }
        public Guid BranchId { get; set; }
    }
}
