using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public EmployeeJobType JobType { get; set; }
        public Guid BranchId { get; set; }

        public VehicleType? VehicleType { get; set; }
        public string? VehiclePlate { get; set; }
        public string? Region { get; set; }

        // Yalnız gösterim: formda "Giriş hesabı var" rozeti. Hesap bilgileri Kullanıcılar ekranından yönetilir
        public bool HasLoginAccount { get; set; }
    }
}
