namespace DestinoTrack.Business.Consts
{
   
    // Business'ta duruyor: kullanıcı servisi (Business) de, controller'lar (WebUI) da görebilsin diye.
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Personel = "Personel";
        public const string Customer = "Customer";
        public const string Courier = "Courier";

        // Seed ve rol listeleri için
        public static readonly string[] All = { Admin, Manager, Personel, Customer, Courier };
    }
}
