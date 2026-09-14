using DestinoTrack.DTO.DTOs.CityDtos;

namespace DestinoTrack.Business.Services.Cities
{
    public interface ICityService
    {
        // Ülke ve ad filtresi isteğe bağlı: ikisi de boşsa bütün şehirler gelir
        Task<List<ResultCityDto>> GetAllAsync(Guid? countryId = null, string? search = null);

        // Şehir listesi (her ülke + şehir sayısı)
        Task<List<CityCountryFilterDto>> GetCountryFiltersAsync();

        Task<UpdateCityDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCityDto createCityDto);
        Task UpdateAsync(UpdateCityDto updateCityDto);
        Task DeleteAsync(Guid id);
    }
}
