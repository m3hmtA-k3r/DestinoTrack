using DestinoTrack.Entity.Entities.Common;

namespace DestinoTrack.Entity.Entities
{
    public class Address : BaseEntity
    {
        public string Title { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string FullAddress { get; set; }

        public Guid UserId { get; set; }

        // Navigation Property
        public virtual AppUser User { get; set; }
    }
}
