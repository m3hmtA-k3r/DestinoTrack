using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities.Enums;

namespace DestinoTrack.Business.Services.Customers
{
    public interface ICustomerService
    {
        // Admin müşteri listesi: durum · tür · arama (ünvan, kod, vergi no)
        Task<PagedResult<ResultCustomerDto>> GetPagedAsync(AccountStatus? status = null, CustomerType? customerType = null,
                                                           string? search = null, int page = 1);

        // Listenin üstündeki durum çipleri — dört durumun hepsi, sayısı sıfır olanlar dahil
        Task<List<CustomerStatusFilterDto>> GetStatusFiltersAsync();

        // Üstteki dört KPI kartı
        Task<CustomerPortfolioDto> GetPortfolioAsync();

        // Düzenleme formunu dolduran kayıt
        Task<UpdateCustomerDto> GetByIdAsync(Guid id);

        // Kod üretilir, durum Active doğar. Vergi numarasının ÜLKEYE göre
        // biçimi ve aynı ülkede tekilliği burada denetlenir — validator veritabanına gidemez
        Task CreateAsync(CreateCustomerDto createCustomerDto);

        // Kod ve durum YAZILMAZ: ikisi de formda salt okunur 
        Task UpdateAsync(UpdateCustomerDto updateCustomerDto);

        // Onayla · askıya al · yeniden aktifleştir · kapat.
        // İzin verilen geçişler kodda sabit, listede olmayan geçiş reddedilir
        Task SetStatusAsync(Guid id, AccountStatus status);
    }
}
