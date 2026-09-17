using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Entity.Entities
{
    // Kim · ne zaman · hangi kayıtta · ne yaptı 
    // BaseEntity'den türemez: denetim kaydı güncellenmez, silinmez ve kendisi denetlenmez
    public class AuditLog
    {
        public Guid Id { get; set; } 

        // İşlemi yapan. Girişsiz işlemde (müşteri kayıt formu, uygulama açılışındaki seed) boş
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }  // o anki e-posta — kullanıcı sonradan değişse de kimin yaptığı okunur

        public AuditAction Action { get; set; }
        public string EntityName { get; set; } = string.Empty;   // "City", "Country" ...
        public Guid EntityId { get; set; }
        public DateTime CreatedDate { get; set; }  // UTC

        public string? Description { get; set; }    // ileride özel işlemler için (kargo durum değişikliği)
        public string? OldValues { get; set; }      // değişen alanların eski değeri
        public string? NewValues { get; set; }      //değişen alanların yeni değeri
    }
}
