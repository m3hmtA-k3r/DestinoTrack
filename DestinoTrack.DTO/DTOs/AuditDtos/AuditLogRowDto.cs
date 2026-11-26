using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.DTO.DTOs.AuditDtos
{
    public class AuditLogRowDto
    {
        public DateTime CreatedDate { get; set; }

        // Kullanıcı sonradan değişse de "kimin yaptığı" okunur kalsın diye e-posta kaydın içinde.
        // Girişsiz işlemde (müşteri kayıt formu, açılış seed'i) boş
        public string? UserEmail { get; set; }

        public AuditAction Action { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }

        // Değişen alanların JSON'ı: yenisi sütunda, eskisi ipucunda
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
