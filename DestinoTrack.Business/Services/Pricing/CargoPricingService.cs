using DestinoTrack.DataAccess.Repositories.Branches;
using DestinoTrack.DataAccess.Repositories.CargoPrices;
using DestinoTrack.DataAccess.Repositories.CargoTypeRates;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DTO.DTOs.PricingDtos;
using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace DestinoTrack.Business.Services.Pricing
{
    public class CargoPricingService(IBranchRepository _branchRepository,
                                     ICountryRepository _countryRepository,
                                     ICargoPriceRepository _cargoPriceRepository,
                                     ICargoTypeRateRepository _cargoTypeRateRepository,
                                     IStringLocalizer<SharedResource> _localizer) : ICargoPricingService
    {
        //3000 cm³ = 1 desi (sektör kabulü) — hacimli ama hafif gönderi ucuza kaçmasın diye
        private const double DesiDivisor = 3000;

        public async Task<CargoQuoteDto> GetQuoteAsync(CargoQuoteRequestDto request)
        {
            // Şehirleriyle okunur: kademe şehir ve ülke karşılaştırmasıyla bulunuyor
            var origin = await _branchRepository.GetWithCityAsync(request.OriginBranchId)
                         ?? throw new ValidationException(_localizer["BranchNotFound"].Value);
            var destination = await _branchRepository.GetWithCityAsync(request.DestinationBranchId)
                              ?? throw new ValidationException(_localizer["BranchNotFound"].Value);

            var desi = Math.Round(request.Width * request.Height * request.Length / DesiDivisor, 2);
            var chargeableUnit = Math.Max((decimal)request.Weight, (decimal)desi);

            // Ağırlık da ölçü de yoksa fiyat sorulamaz; eksi değer zaten hatalı giriş
            if (chargeableUnit <= 0 || request.Weight < 0 || request.Width < 0 || request.Height < 0 || request.Length < 0)
            {
                throw new ValidationException(_localizer["MeasureInvalid"].Value);
            }

            var routeScope = GetRouteScope(origin, destination);

            //tarife ve para birimi ÇIKIŞ ülkesinden
            var countryId = origin.City.CountryId;
            var country = await _countryRepository.GetByIdAsync(countryId)
                          ?? throw new ValidationException(_localizer["CountryNotFound"].Value);

            var tariff = await _cargoPriceRepository.GetByCountryAndScopeAsync(countryId, routeScope)
                         ?? throw new ValidationException(_localizer["PriceRuleNotFound"].Value);
            var typeRate = await _cargoTypeRateRepository.GetByTypeAsync(request.CargoType)
                           ?? throw new ValidationException(_localizer["CargoTypeRateNotFound"].Value);

            var amount = (tariff.BasePrice + chargeableUnit * tariff.PricePerDesi) * typeRate.Multiplier;

            // tip gün farkı eklenir, sonuç en az 1 gün (Acil kademeyi sıfıra indirmesin)
            var transitDays = Math.Max(1, tariff.TransitDays + typeRate.TransitDaysDelta);
            var shipmentDate = request.ShipmentDate ?? DateTime.UtcNow;

            return new CargoQuoteDto
            {
                Price = Math.Round(amount, 2, MidpointRounding.AwayFromZero),
                CurrencyCode = country.CurrencyCode,
                EstimatedArrivalDate = shipmentDate.AddDays(transitDays),
                TransitDays = transitDays,
                RouteScope = routeScope,
                Desi = desi,
                ChargeableUnit = chargeableUnit,
                BasePrice = tariff.BasePrice,
                PricePerDesi = tariff.PricePerDesi,
                TypeMultiplier = typeRate.Multiplier
            };
        }

        // aynı şehir → aynı ülke → yurt dışı
        private static RouteScope GetRouteScope(Branch origin, Branch destination)
        {
            if (origin.CityId == destination.CityId)
            {
                return RouteScope.SameCity;
            }

            return origin.City.CountryId == destination.City.CountryId
                ? RouteScope.SameCountry
                : RouteScope.International;
        }
    }
}
