using DestinoTrack.Business.Consts;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DTO.DTOs.Common;
using DestinoTrack.DTO.DTOs.CountryDtos;
using DestinoTrack.Entity.Entities;
using Mapster;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Countries
{
    public class CountryService(ICountryRepository _countryRepository, 
                                ICurrentUserAccessor _currentUser, IStringLocalizer<SharedResource> _localizer) : ICountryService
    {
        public async Task<List<ResultCountryDto>> GetAllAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            return countries.Adapt<List<ResultCountryDto>>();
        }

        public async Task<PagedResult<ResultCountryDto>> GetPagedAsync(int page = 1)
        {
            page = page < 1 ? 1 : page;

            var (countries, totalCount) = await _countryRepository.GetPagedAsync(page, Paging.PageSize);

            if (countries.Count == 0 && totalCount > 0)
            {
                page = (int)Math.Ceiling(totalCount / (double)Paging.PageSize);
                (countries, totalCount) = await _countryRepository.GetPagedAsync(page, Paging.PageSize);
            }

            return new PagedResult<ResultCountryDto>
            {
                Items = countries.Adapt<List<ResultCountryDto>>(),
                Page = page,
                PageSize = Paging.PageSize,
                TotalCount = totalCount
            };
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


        // Manager paneli için
        // Ülke panelinin başlığı — kimlik oturumdan okunur, çağırandan alınmaz
        public async Task<CountryHeaderDto?> GetMyPanelHeaderAsync()
        {
            // Ülkesi atanmamış kullanıcı ya da girişsiz istek: veritabanına hiç gidilmez
            if (_currentUser.CountryId is null)
            {
                return null;
            }

            var header = await _countryRepository.GetPanelHeaderAsync(_currentUser.CountryId.Value);

            // Ülkesi atanmış ama kayıt silinmiş: ekran "atanmamış" hâliyle aynı davranır
            if (header is null)
            {
                return null;
            }

            return new CountryHeaderDto
            {
                Name = header.Value.Name,
                BranchCount = header.Value.BranchCount
            };
        }
    }
}
