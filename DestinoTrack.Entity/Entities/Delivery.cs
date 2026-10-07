using DestinoTrack.Entity.Entities.Common;

namespace DestinoTrack.Entity.Entities
{
    //  başarılı teslim kaydı — kargo başına tek satır
    public class Delivery : BaseEntity
    {
        public Guid CargoId { get; set; }
        public string Code { get; set; }

        // Teslim alan kişi: alıcının kendisi olmayabilir (komşu, kapıcı, iş yeri)
        public string RecipientName { get; set; }
        public Guid DeliveredByEmployeeId { get; set; }

        public DateTime DeliveredAt { get; set; }

        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
        public virtual Employee DeliveredByEmployee { get; set; }
    }
}
