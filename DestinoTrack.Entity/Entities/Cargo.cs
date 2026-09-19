using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    public class Cargo : BaseEntity
    {
        public string TrackCode { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public double Weight { get; set; }

        public CargoType CargoType { get; set; }
        public CargoStatus CargoStatus { get; set; }

        // Ücretlendirme
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
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

        // Kuryelik yapan personel (Employee.JobType = Courier)
        public virtual Employee Courier { get; set; }
        public virtual IList<CargoMovement> Movements { get; set; }
        public virtual IList<Payment> Payments { get; set; }

        //Kargo ücreti, ağırlık ile desinin büyüğü üzerinden hesaplanır.
        //Şu an Cargo'da sadece Weight var — hacimli ama hafif gönderiler
        //(yastık, koltuk, abajur) olduğundan çok ucuza fiyatlanır
        public double Width { get; set; }    // cm
        public double Height { get; set; }   // cm
        public double Length { get; set; }   // cm
        public double Desi { get; set; }     // (W × H × L) / 3000
        public string Barcode { get; set; }  // takip kodundan ayrı barkod

        public Guid? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
