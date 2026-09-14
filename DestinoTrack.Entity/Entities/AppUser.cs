using DestinoTrack.Entity.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public Guid? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Kullanıcı kapsamı Admin ve Customer'da ikisi de boş
        public Guid? CountryId { get; set; }   // Manager: sorumlu olduğu ülke
        public Guid? BranchId { get; set; }    // Personel / Courier: çalıştığı şube

        // Navigation Properties
        public virtual Country Country { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual IList<Cargo> SentCargos { get; set; }
        public virtual IList<Cargo> ReceivedCargos { get; set; }
        public virtual IList<Address> Addresses { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
