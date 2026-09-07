using DestinoTrack.Entity.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.Entity.Entities
{
    public class AppUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation Properties
        public virtual IList<Cargo> SentCargos { get; set; }
        public virtual IList<Cargo> ReceivedCargos { get; set; }
        public virtual IList<Address> Addresses { get; set; }
    }
}
