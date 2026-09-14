namespace DestinoTrack.Business.Consts
{
   
    // Business'ta duruyor: kullanıcı servisi (Business) de yaptım, controller'lar (WebUI) da görebilsin diye.
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Personel = "Personel";
        public const string Customer = "Customer";
        public const string Courier = "Courier";

        // Seed ve rol listeleri için
        public static readonly string[] All = { Admin, Manager, Personel, Customer, Courier };

        // Admin kullanıcı yönetiminde seçilebilen roller — Customer yalnızca kayıt formundan açılır 
        public static readonly string[] Staff = { Admin, Manager, Personel, Courier };
    }
}
