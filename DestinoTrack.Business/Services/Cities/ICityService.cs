using DestinoTrack.DTO.DTOs.CityDtos;

namespace DestinoTrack.Business.Services.Cities
{
    public interface ICityService
    {
        Task<List<ResultCityDto>> GetAllAsync();
        Task<UpdateCityDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateCityDto createCityDto);
        Task UpdateAsync(UpdateCityDto updateCityDto);
        Task DeleteAsync(Guid id);
    }
}
