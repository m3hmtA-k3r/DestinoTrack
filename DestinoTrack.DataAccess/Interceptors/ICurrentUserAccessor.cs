namespace DestinoTrack.DataAccess.Interceptors
{
    // İşlemi yapan kullanıcı — DataAccess HttpContext'i bilmez; WebUI doldurur (katmanlar karışmaz)
    public interface ICurrentUserAccessor
    {
        Guid? UserId { get; }
        string? Email { get; }

        //Veri kapsamı — Admin hepsini, Personel şubesini, Manager ülkesini, Müşteri kendisini görür
        string? Role { get; }
        Guid? BranchId { get; }
        Guid? CountryId { get; }
        Guid? CustomerId { get; }
    }
}
