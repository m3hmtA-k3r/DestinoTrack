using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{//Kargonun takip geçmişi
    public class CargoMovement: BaseEntity
    {
        public DateTime MovementDate { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public string Description { get; set; }


        // Foreign Keys
        public Guid CargoId { get; set; }
        public Guid BranchId { get; set; }


        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
        public virtual Branch Branch { get; set; }

        public DelayReason? DelayReason { get; set; } // gecikme yoksa boş kalır
    }
}
