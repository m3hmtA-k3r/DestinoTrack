using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    // başarısız teslim denemesi — kargo başına birden çok satır olabilir
    public class DeliveryException : BaseEntity
    {
        public Guid CargoId { get; set; }

        // Kaçıncı deneme: 1, 2, 3... Sınıra ulaşınca iade süreci başlar (sınır appsettings'ten)
        public int AttemptNo { get; set; }

        public DeliveryFailureReason Reason { get; set; }

        // Gerekçe "Diğer" ise zorunlu; diğerlerinde kuryenin notu
        public string? Description { get; set; }
        public Guid AttemptedByEmployeeId { get; set; }

        public DateTime AttemptedAt { get; set; }

        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
        public virtual Employee AttemptedByEmployee { get; set; }
    }
}
