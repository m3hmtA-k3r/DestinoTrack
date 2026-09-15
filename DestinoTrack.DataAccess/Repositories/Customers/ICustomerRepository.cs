using DestinoTrack.DataAccess.Repositories.GenericRepositories;
using DestinoTrack.Entity.Entities;

namespace DestinoTrack.DataAccess.Repositories.Customers
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        // aynı ülkede aynı kimlik / vergi numarasıyla ikinci müşteri açılamaz
        Task<bool> TaxNumberExistsAsync(Guid countryId, string taxNumber);



        // Otomatik üretilen müşteri kodu daha önce kullanılmış mı (Code tekil)
        Task<bool> CodeExistsAsync(string code);
    }
}
