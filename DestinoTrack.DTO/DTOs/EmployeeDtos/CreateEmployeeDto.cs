using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public EmployeeJobType JobType { get; set; } = EmployeeJobType.BranchStaff;
        public Guid BranchId { get; set; }

        // Kurye alanları — yalnızca JobType = Courier iken zorunlu
        public VehicleType? VehicleType { get; set; }
        public string? VehiclePlate { get; set; }
        public string? Region { get; set; }

        // Giriş hesabı — isteğe bağlı. CreateAccount işaretli değilse aşağıdakiler yok sayılır
        public bool CreateAccount { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? AccountRole { get; set; }   // RoleNames.Personel · RoleNames.Courier
    }
}
