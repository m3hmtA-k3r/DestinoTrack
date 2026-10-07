using DestinoTrack.DTO.DTOs.PricingDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Pricing
{
    public class CreateCargoPriceValidator : AbstractValidator<CreateCargoPriceDto>
    {
        public CreateCargoPriceValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequired"].Value);

            RuleFor(x => x.RouteScope)
                .IsInEnum().WithMessage(localizer["RouteScopeInvalid"].Value);

            // Taban ücret sıfır olabilir (yalnız desi üzerinden fiyatlayan tarife), eksi olamaz
            RuleFor(x => x.BasePrice)
                .InclusiveBetween(0, 1_000_000).WithMessage(localizer["PriceRange"].Value);

            RuleFor(x => x.PricePerDesi)
                .InclusiveBetween(0, 1_000_000).WithMessage(localizer["PriceRange"].Value);

            RuleFor(x => x.TransitDays)
                .InclusiveBetween(1, 90).WithMessage(localizer["TransitDaysRange"].Value);
        }
    }
}
