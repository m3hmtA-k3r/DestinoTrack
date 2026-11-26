using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    // Araç havuzu: araç artık kuryenin bir alanı değil, kendi kaydı olan varlık 
    // Aynı araç birden çok kuryede olabilir — vardiya paylaşımı engellenmez 
    public class Vehicle : BaseEntity
    {
        public string Plate { get; set; }
        public VehicleType VehicleType { get; set; }
        public Guid BranchId { get; set; }
        public bool IsActive { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual IList<Employee> Couriers { get; set; }
    }
}
