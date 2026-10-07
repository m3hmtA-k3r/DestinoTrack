using DestinoTrack.DTO.DTOs.PricingDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Pricing
{
    // Satır içi düzenlenen sekiz satırın her biri için çalışır
    public class CargoTypeRateValidator : AbstractValidator<CargoTypeRateDto>
    {
        public CargoTypeRateValidator(IStringLocalizer<SharedResource> localizer)
        {
            // 0,10 = %90 indirim · 10,00 = 10 kat. Dışı yanlış giriştir
            RuleFor(x => x.Multiplier)
                .InclusiveBetween(0.10m, 10m).WithMessage(localizer["MultiplierRange"].Value);

            // Kademenin gününü en fazla beş gün kısaltır ya da uzatır
            RuleFor(x => x.TransitDaysDelta)
                .InclusiveBetween(-5, 5).WithMessage(localizer["TransitDaysDeltaRange"].Value);
        }
    }
}
