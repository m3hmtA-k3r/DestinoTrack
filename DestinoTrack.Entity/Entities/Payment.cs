using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string ReferenceNo { get; set; }

        // Foreign Keys
        public Guid CargoId { get; set; }
        public Guid? CollectedByCourierId { get; set; }
        public Guid? CollectedByBranchId { get; set; }

        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
        public virtual Courier CollectedByCourier { get; set; }
        public virtual Branch CollectedByBranch { get; set; }
    }
}
