using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DTO.DTOs.CountryDtos;
using DestinoTrack.Entity.Entities;
using Mapster;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Countries
{
    public class CountryService(ICountryRepository _countryRepository, IStringLocalizer<SharedResource> _localizer) : ICountryService
    {
        public async Task<List<ResultCountryDto>> GetAllAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            return countries.Adapt<List<ResultCountryDto>>();
        }

        public async Task<UpdateCountryDto> GetByIdAsync(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
            {
                throw new ValidationException(_localizer["CountryNotFound"].Value);
            }

            return country.Adapt<UpdateCountryDto>();
        }

        public async Task CreateAsync(CreateCountryDto createCountryDto)
        {
            var country = createCountryDto.Adapt<Country>();
            await _countryRepository.CreateAsync(country);
        }

        public async Task UpdateAsync(UpdateCountryDto updateCountryDto)
        {
            var country = await _countryRepository.GetByIdAsync(updateCountryDto.Id);
            if (country == null)
            {
                throw new ValidationException(_localizer["CountryNotFound"].Value);
            }

            updateCountryDto.Adapt(country);
            await _countryRepository.UpdateAsync(country);
        }

        public async Task DeleteAsync(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
            {
                throw new ValidationException(_localizer["CountryNotFound"].Value);
            }

            // Bağlı şehir / müşteri / kullanıcı varken silinmez (soft delete'te bunu veritabanı engellemiyor)
            if (await _countryRepository.HasDependentsAsync(id))
            {
                throw new ValidationException(_localizer["CountryHasDependents"].Value);
            }

            await _countryRepository.DeleteAsync(country);
        }

    }
}
