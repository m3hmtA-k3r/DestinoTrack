namespace DestinoTrack.DataAccess.Interceptors
{
    // İşlemi yapan kullanıcı — DataAccess HttpContext'i bilmez; WebUI doldurur (katmanlar karışmaz)
    public interface ICurrentUserAccessor
    {
        Guid? UserId { get; }
        string? Email { get; }
    }
}
