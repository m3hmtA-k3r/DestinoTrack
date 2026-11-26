using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.EmployeeDtos
{
    // Admin → Kurye & Dağıtım ekranının bir satırı.  
    public class CourierOverviewDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }

        public Guid BranchId { get; set; }
        public string BranchName { get; set; }

        // Araç havuzdan geliyor; kuryenin aracı yoksa ikisi de boş 
        public VehicleType? VehicleType { get; set; }
        public string? VehiclePlate { get; set; }

        public string? Region { get; set; }
        public decimal? Rating { get; set; }

        // Şu an üzerindeki kargo (OutForDelivery) ve bugün teslim ettiği
        public int OnDeliveryCount { get; set; }
        public int DeliveredTodayCount { get; set; }
    }
}
