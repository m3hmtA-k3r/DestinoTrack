using DestinoTrack.Entity.Entities.Common;

namespace DestinoTrack.Entity.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; }

        public Guid CountryId { get; set; }

        // Navigation Properties
        public virtual Country Country { get; set; }
        public virtual IList<Branch> Branches { get; set; }
    }
}
