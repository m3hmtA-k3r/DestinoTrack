using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities.Enums;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Customers
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        // Aynı ülkede aynı kimlik / vergi numarasıyla ikinci müşteri açılamaz.
        // excludeCustomerId: düzenlemede kaydın KENDİSİ çakışma sayılmasın (kayıt formunda null)
        Task<bool> TaxNumberExistsAsync(Guid countryId, string taxNumber, Guid? excludeCustomerId = null);



        // Otomatik üretilen müşteri kodu daha önce kullanılmış mı (Code tekil)
        Task<bool> CodeExistsAsync(string code);

        // Gişe formundaki açılır liste, iki süzgeç: askıdaki müşteri çıkmaz (CreateAsync onu zaten
        // reddediyor,) · yalnız paketin teslim alındığı ülkenin müşterileri 
        Task<List<Customer>> GetActiveByCountryAsync(Guid countryId);


        // Admin müşteri listesi: durum · tür · arama (ünvan, kod, vergi no) + sayfalama.
        // Onay bekleyenler başta — işlem bekleyen kayıt listenin dibinde kaybolmasın
        Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(AccountStatus? status, CustomerType? customerType,
                                                                   string? search, int page, int pageSize);

        // Yalnız o SAYFADAKİ müşterilerin gönderi sayısı ve son gönderi tarihi.
        // Ayrı sorgu çünkü satır başına Cargos'a girmek her müşteri için bir sorgu daha açardı (N+1)
        Task<Dictionary<Guid, (int CargoCount, DateTime? LastCargoDate)>> GetCargoStatsAsync(List<Guid> customerIds);


        // Durum çipleri: "Aktif 120 · Onay bekliyor 14 · Askıda 3 · Kapalı 0".
        // Toplam müşteri ve onay bekleyen sayısı BURADAN türetilir, ayrıca sayılmaz
        Task<Dictionary<AccountStatus, int>> GetCountsByStatusAsync();

        // KPI kartlarının durum sayımlarından TÜRETİLEMEYEN ikisi
        Task<(int ActiveCorporate, int NewThisMonth)> GetPortfolioCountsAsync();
    }
}
