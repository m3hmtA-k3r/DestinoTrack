using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    public class Employee : BaseEntity
    {// Şubede çalışan personel. Kurye de bir personeldir (JobType = Courier)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public EmployeeJobType JobType { get; set; }

        // Çalıştığı şube
        public Guid BranchId { get; set; }

        // Sisteme giriş yapan personelin kullanıcı hesabı. Depo görevlisi gibi giriş yapmayanlarda boş
        public Guid? AppUserId { get; set; }

        // Yalnızca JobType = Courier olan personelde dolu
        public VehicleType? VehicleType { get; set; }
        public string? VehiclePlate { get; set; }
        public string? Region { get; set; }
        public decimal? Rating { get; set; }

        // Navigation Properties
        public virtual Branch Branch { get; set; }
        public virtual AppUser AppUser { get; set; }

        // Kurye olarak taşıdığı kargolar ve tahsil ettiği ödemeler
        public virtual IList<Cargo> Cargos { get; set; }
        public virtual IList<Payment> CollectedPayments { get; set; }
    }
}
