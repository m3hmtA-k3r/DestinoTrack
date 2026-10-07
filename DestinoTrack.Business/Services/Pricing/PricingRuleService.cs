using DestinoTrack.DataAccess.Repositories.CargoPrices;
using DestinoTrack.DataAccess.Repositories.CargoTypeRates;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Mapster;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Pricing
{
    public class PricingRuleService(ICargoPriceRepository _cargoPriceRepository,
                                    ICargoTypeRateRepository _cargoTypeRateRepository,
                                    IStringLocalizer<SharedResource> _localizer) : IPricingRuleService
    {
        public async Task<List<ResultCargoPriceDto>> GetTariffsAsync()
        {
            var tariffs = await _cargoPriceRepository.GetAllWithCountryAsync();

            return tariffs.Select(p => new ResultCargoPriceDto
            {
                Id = p.Id,
                CountryId = p.CountryId,
                CountryName = p.Country?.Name,
                CurrencyCode = p.Country?.CurrencyCode,   // para birimi ülkeden
                RouteScope = p.RouteScope,
                BasePrice = p.BasePrice,
                PricePerDesi = p.PricePerDesi,
                TransitDays = p.TransitDays
            }).ToList();
        }

        public async Task<UpdateCargoPriceDto> GetTariffByIdAsync(Guid id)
        {
            var tariff = await FindTariffAsync(id);
            return tariff.Adapt<UpdateCargoPriceDto>();
        }

        public async Task CreateTariffAsync(CreateCargoPriceDto createCargoPriceDto)
        {
            // Tekil indeks veritabanında da koruyor; buradaki denetim kullanıcıya anlaşılır mesaj için
            await EnsureUniqueAsync(createCargoPriceDto.CountryId, createCargoPriceDto.RouteScope);

            var tariff = createCargoPriceDto.Adapt<CargoPrice>();
            await _cargoPriceRepository.CreateAsync(tariff);
        }

        public async Task UpdateTariffAsync(UpdateCargoPriceDto updateCargoPriceDto)
        {
            var tariff = await FindTariffAsync(updateCargoPriceDto.Id);

            await EnsureUniqueAsync(updateCargoPriceDto.CountryId, updateCargoPriceDto.RouteScope, tariff.Id);

            updateCargoPriceDto.Adapt(tariff);
            await _cargoPriceRepository.UpdateAsync(tariff);
        }

        public async Task DeleteTariffAsync(Guid id)
        {
            // Kargo kendi ücretini kopyalayarak saklar; tarife silinince eski kargolar etkilenmez
            var tariff = await FindTariffAsync(id);
            await _cargoPriceRepository.DeleteAsync(tariff);
        }

        public async Task<List<CargoTypeRateDto>> GetRatesAsync()
        {
            var rates = await _cargoTypeRateRepository.GetAllOrderedAsync();
            return rates.Adapt<List<CargoTypeRateDto>>();
        }

        public async Task UpdateRatesAsync(List<CargoTypeRateDto> rates)
        {
            var current = await _cargoTypeRateRepository.GetAllOrderedAsync();

            // Yalnız var olan satırlar güncellenir: tipler enum'dan geliyor, ekleme/silme yok
            foreach (var rate in rates)
            {
                var entity = current.FirstOrDefault(r => r.Id == rate.Id);
                if (entity == null)
                {
                    continue;
                }

                entity.Multiplier = rate.Multiplier;
                entity.TransitDaysDelta = rate.TransitDaysDelta;
                await _cargoTypeRateRepository.UpdateAsync(entity);
            }
        }

        private async Task EnsureUniqueAsync(Guid countryId, RouteScope routeScope, Guid? currentId = null)
        {
            var existing = await _cargoPriceRepository.GetByCountryAndScopeAsync(countryId, routeScope);
            if (existing != null && existing.Id != currentId)
            {
                throw new ValidationException(_localizer["PriceRuleExists"].Value);
            }
        }

        private async Task<CargoPrice> FindTariffAsync(Guid id)
        {
            return await _cargoPriceRepository.GetByIdAsync(id)
                   ?? throw new ValidationException(_localizer["PriceRuleNotFound"].Value);
        }
    }
}
