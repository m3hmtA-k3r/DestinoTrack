using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;
using System.Diagnostics.Metrics;

namespace DestinoTrack.Entity.Entities
{
    public class Customer: BaseEntity
    {
        public string Code {  get; set; }
        public string Title { get; set; } // kurumsalda ünvan, bireyselde ad soyad
        public CustomerType CustomerType { get; set; } // Bireysel / Kurumsal
        public string TaxNumber { get; set; }//  vergi no veya TCKN
        public string TaxOffice {  get; set; } //Vergi dairesi
        public string Email {  get; set; }
        public string PhoneNumber { get; set; }
        public AccountStatus Status { get; set; } // AccountType
        public decimal CreditLimit { get; set; } // kota
        public decimal CurrentBalance { get; set; }// cari bakiye 


        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }


        public Guid CountryId { get; set; }

        // Navigation Properties
        public virtual Country Country { get; set; }
        public virtual IList<Cargo> Cargos { get; set; }
        public virtual IList<AppUser> Users { get; set; }




    }
}
