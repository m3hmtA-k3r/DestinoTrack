using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.EmployeeDtos
{
    public class ResultEmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public EmployeeJobType JobType { get; set; }

        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        public string CityName { get; set; }

        // Kurye alanları — kurye değilse boş
        public VehicleType? VehicleType { get; set; }
        public string? VehiclePlate { get; set; }

        // formda yok, yalnız listede; puan yoksa ekranda "—"
        public decimal? Rating { get; set; }

        // bağlı giriş hesabı var mı (listede rozet)
        public bool HasLoginAccount { get; set; }
    }
}
