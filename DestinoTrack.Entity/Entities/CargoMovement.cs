using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    public class CargoMovement : BaseEntity  //Kargonun takip geçmişi
    {
        public DateTime MovementDate { get; set; }

        // Durum geçişi: OldStatus → CargoStatus  
        public CargoStatus? OldStatus { get; set; }    // ilk harekette (kargo oluşturuldu) önceki durum yok → boş
        public CargoStatus CargoStatus { get; set; }   // hareketten sonraki, yani yeni durum
        public string Description { get; set; }


        // Foreign Keys
        public Guid CargoId { get; set; }
        public Guid BranchId { get; set; }
        public Guid PerformedByUserId { get; set; }    // işlemi yapan kullanıcı 


        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual AppUser PerformedByUser { get; set; }

        public DelayReason? DelayReason { get; set; } // gecikme yoksa boş kalır
    }
}
