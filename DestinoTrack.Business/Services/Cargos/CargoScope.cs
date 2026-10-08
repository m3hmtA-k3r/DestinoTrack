namespace DestinoTrack.Business.Services.Cargos
{
    //Kullanıcının görebileceği kargoların sınırı.
    // Dolu olan alan sorguyu daraltır; hepsi boşsa (IsUnrestricted) sınır yok demektir
    public class CargoScope
    {
        // Admin: sınırsız
        public bool IsUnrestricted { get; init; }

        // Personel / Kurye: kendi şubesinden çıkan ya da kendi şubesine gelen kargolar
        public Guid? BranchId { get; init; }

        // Manager: kendi ülkesindeki şubeler arasında taşınan kargolar
        public Guid? CountryId { get; init; }

        // Kurumsal müşteri çalışanı: şirketinin kargoları
        public Guid? CustomerId { get; init; }

        // Bireysel müşteri: gönderici ya da alıcı olduğu kargolar
        public Guid? UserId { get; init; }

        // Hiçbir şey göremeyen kullanıcı (giriş yok ya da kapsamı belirsiz): sorgu boş dönmeli
        public bool IsEmpty => !IsUnrestricted && BranchId is null && CountryId is null && CustomerId is null && UserId is null;
    }
}
