using DestinoTrack.Entity.Entities.Common;

namespace DestinoTrack.Entity.Entities
{
    public class Courier: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public Guid BranchId { get; set; }

        // Navigation Properties
        public virtual Branch Branch { get; set; }
        public virtual IList<Cargo> Cargos { get; set; }


        // Courier için
        public virtual IList<Payment> CollectedPayments { get; set; }
    }
}
