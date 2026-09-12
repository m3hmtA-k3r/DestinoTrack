using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

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

        public VehicleType VehicleType { get; set; }  // Motorize / Panelvan / Kamyonet
        public string VehiclePlate { get; set; }      // Plakası
        public string Region { get; set; }            // dağıtım bölgesi
        public decimal Rating { get; set; }           // müşteri puanı, 0-5
    }
}
