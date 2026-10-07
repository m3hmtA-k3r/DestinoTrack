using DestinoTrack.DTO.DTOs.PricingDtos;

namespace DestinoTrack.Business.Services.Pricing
{
    public interface ICargoPricingService
    {
        //ücret ve tahmini teslim tarihi. Kargo kaydı açılmadan da sorulabilir
        Task<CargoQuoteDto> GetQuoteAsync(CargoQuoteRequestDto request);
    }
}
