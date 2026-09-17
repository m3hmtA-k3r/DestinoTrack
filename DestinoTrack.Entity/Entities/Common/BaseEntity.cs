namespace DestinoTrack.Entity.Entities.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        // Tarihler UTC 
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Soft delete silinen kayıt veritabanında kalır, sadece sorgulardan gizlenir
        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
