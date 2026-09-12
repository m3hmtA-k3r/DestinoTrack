using DestinoTrack.DTO.DTOs.CountryDtos;

namespace DestinoTrack.Business.Services.Countries
{
    public interface ICountryService
    {
        Task<List<ResultCountryDto>> GetAllAsync();
        Task<UpdateCountryDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCountryDto createCountryDto);
        Task UpdateAsync(UpdateCountryDto updateCountryDto);
        Task DeleteAsync(Guid id);
    }
}
