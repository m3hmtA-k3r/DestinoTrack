using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    public class Branch: BaseEntity
    {
        public string Name { get; set; }

        public Guid CityId { get; set; }

        // Navigation Properties
        public virtual City City { get; set; }
        public virtual IList<Cargo> OriginCargos { get; set; }
        public virtual IList<Cargo> DestinationCargos { get; set; }
        public virtual IList<Courier> Couriers { get; set; }

        // Branch için
        public virtual IList<Payment> CollectedPayments { get; set; }


        public string Code { get; set; }             
        public BranchType BranchType { get; set; }    // Şube / Transfer Merkezi / Ana Depo / Lojistik Park
        public int Capacity { get; set; }             // toplam kapasite
        public int CurrentLoad { get; set; }          // anlık doluluk
        public int DockCount { get; set; }            // rampa & hat sayısı
        public Guid? ManagerId { get; set; }          // AppUser ilişkisi

        // Navigation
        public virtual AppUser Manager { get; set; }
    }
}
