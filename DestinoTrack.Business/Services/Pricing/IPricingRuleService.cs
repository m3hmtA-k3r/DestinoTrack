using DestinoTrack.DTO.DTOs.PricingDtos;

namespace DestinoTrack.Business.Services.Pricing
{
    // Fiyat kuralları ekranı: tarifeler + kargo tipi çarpanları tek ekrandan yönetilir
    public interface IPricingRuleService
    {
        Task<List<ResultCargoPriceDto>> GetTariffsAsync();
        Task<UpdateCargoPriceDto> GetTariffByIdAsync(Guid id);
        Task CreateTariffAsync(CreateCargoPriceDto createCargoPriceDto);
        Task UpdateTariffAsync(UpdateCargoPriceDto updateCargoPriceDto);
        Task DeleteTariffAsync(Guid id);

        // Sekiz satır birlikte gelir, birlikte kaydedilir
        Task<List<CargoTypeRateDto>> GetRatesAsync();
        Task UpdateRatesAsync(List<CargoTypeRateDto> rates);
    }
}
