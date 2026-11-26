using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.VehicleDtos
{
    public class CreateVehicleDto
    {
        public string Plate { get; set; }

        // Boş geçilebilir ki "seçim yapılmadı" hâli temsil edilebilsin:
        // aksi hâlde enum sessizce 0'a düşer ve validator'ın söyleyecek sözü kalmaz
        public VehicleType? VehicleType { get; set; }

        public Guid BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}
