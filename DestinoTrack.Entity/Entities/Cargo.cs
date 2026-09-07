using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    public class Cargo: BaseEntity
    {
        public string TrackCode { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public double Weight { get; set; }

        public CargoType CargoType { get; set; }
        public CargoStatus CargoStatus { get; set; }


        // Ücretlendirme
        public decimal Price { get; set; }
        public bool IsPaid { get; set; }
        public PaymentType PaymentType { get; set; }


        // Foreign Keys
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }
        public Guid? CourierId { get; set; }


        // Navigation Properties
        public virtual AppUser Sender { get; set; }
        public virtual AppUser Receiver { get; set; }
        public virtual Branch OriginBranch { get; set; }
        public virtual Branch DestinationBranch { get; set; }
        public virtual Courier Courier { get; set; }
        public virtual IList<CargoMovement> Movements { get; set; }
        public virtual IList<Payment> Payments { get; set; }


    }
}