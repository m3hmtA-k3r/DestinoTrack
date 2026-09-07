using DestinoTrack.Entity.Entities.Common;

namespace DestinoTrack.Entity.Entities
{
    public class City: BaseEntity
    {
        public string Name { get; set; }

        // Navigation Property
        public virtual IList<Branch> Branches { get; set; }
    }
}
