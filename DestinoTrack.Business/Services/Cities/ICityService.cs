using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.DTO.DTOs.Common;

namespace DestinoTrack.Business.Services.Cities
{
    public interface ICityService
    {
        // Sayfalı liste: filtreler aynı, ek olarak sayfa numarası
        Task<PagedResult<ResultCityDto>> GetPagedAsync(Guid? countryId = null, string? search = null, int page = 1);

        // Şehir listesi (her ülke + şehir sayısı)
        Task<List<CityCountryFilterDto>> GetCountryFiltersAsync();

        Task<List<CityLookupDto>> GetLookupAsync();   // sayfasız, yalnızca Id + ad + ülke


        Task<UpdateCityDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCityDto createCityDto);
        Task UpdateAsync(UpdateCityDto updateCityDto);
        Task DeleteAsync(Guid id);
    }
}
