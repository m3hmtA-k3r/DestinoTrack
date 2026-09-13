using DestinoTrack.DataAccess.Repositories.Cities;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DTO.DTOs.CityDtos;
using DestinoTrack.Entity.Entities;
using Mapster;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Cities
{
    public class CityService(ICityRepository _cityRepository, ICountryRepository _countryRepository, IStringLocalizer<SharedResource> _localizer) : ICityService
    {
        public async Task<List<ResultCityDto>> GetAllAsync(Guid? countryId = null, string? search = null)
        {
            var cities = await _cityRepository.GetAllWithCountryAsync(countryId, search);

            return cities.Select(c => new ResultCityDto
            {
                Id = c.Id,
                Name = c.Name,
                CountryId = c.CountryId,
                CountryName = c.Country?.Name
            }).ToList();
        }

        // Şehri olmayan ülke de çip olarak görünür (sayısı 0) —
        // bu yüzden sayımlar ülke listesinin üzerine eklenir, tersi değil
        public async Task<List<CityCountryFilterDto>> GetCountryFiltersAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            var counts = await _cityRepository.GetCityCountsByCountryAsync();

            return countries
                .OrderBy(c => c.Name)
                .Select(c => new CityCountryFilterDto
                {
                    CountryId = c.Id,
                    CountryName = c.Name,
                    CityCount = counts.GetValueOrDefault(c.Id)
                })
                .ToList();
        }

        public async Task<UpdateCityDto> GetByIdAsync(Guid id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                throw new ValidationException(_localizer["CityNotFound"].Value);
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
                throw new ValidationException(_localizer["CityNotFound"].Value);
            }

            updateCityDto.Adapt(city);
            await _cityRepository.UpdateAsync(city);
        }

        public async Task DeleteAsync(Guid id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                throw new ValidationException(_localizer["CityNotFound"].Value);
            }

            await _cityRepository.DeleteAsync(city);
        }
    }
}
