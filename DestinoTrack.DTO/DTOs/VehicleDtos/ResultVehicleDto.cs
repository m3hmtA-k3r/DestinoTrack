using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.VehicleDtos
{
    public class ResultVehicleDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; }
        public VehicleType VehicleType { get; set; }

        public Guid BranchId { get; set; }
        public string BranchName { get; set; }

        public bool IsActive { get; set; }

        // Aracı kullanan kurye sayısı: havuzda bir araç birden çok kuryede olabilir 
        // silme koruması da bu sayıdan okunur 
        public int CourierCount { get; set; }
    }
}
