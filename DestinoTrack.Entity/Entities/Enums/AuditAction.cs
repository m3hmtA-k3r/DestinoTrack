namespace DestinoTrack.Entity.Entities.Enums
{
    // AuditLog işlem türü 
    public enum AuditAction
    {
        Create = 1,
        Update = 2,
        Delete = 3   // soft delete dahil: kayıt silinmiş işaretlense de denetimde "silme"dir
    }
}
