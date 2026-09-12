using DestinoTrack.DataAccess.Repositories.Cities;
using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.Entity.Entities;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Cities
{
    public class CityService(ICityRepository _cityRepository) : ICityService
    {
        public async Task<List<ResultCityDto>> GetAllAsync()
        {
            var cities = await _cityRepository.GetAllWithCountryAsync();

            return cities.Select(c => new ResultCityDto
            {
                Id = c.Id,
                Name = c.Name,
                CountryId = c.CountryId,
                CountryName = c.Country?.Name
            }).ToList();
        }

        public async Task<UpdateCityDto> GetByIdAsync(Guid id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                throw new ValidationException("Şehir bulunamadı.");
            }

            return city.Adapt<UpdateCityDto>();
        }

        public async Task CreateAsync(CreateCityDto createCityDto)
        {
            var city = createCityDto.Adapt<City>();
            await _cityRepository.CreateAsync(city);
        }

        public async Task UpdateAsync(UpdateCityDto updateCityDto)
        {
            var city = await _cityRepository.GetByIdAsync(updateCityDto.Id);
            if (city == null)
            {
                throw new ValidationException("Şehir bulunamadı.");
            }

            updateCityDto.Adapt(city);
            await _cityRepository.UpdateAsync(city);
        }

        public async Task DeleteAsync(Guid id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                throw new ValidationException("Şehir bulunamadı.");
            }

            await _cityRepository.DeleteAsync(city);
        }
    }
}
