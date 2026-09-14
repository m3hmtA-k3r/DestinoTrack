using DestinoTrack.DTO.DTOs.CountryDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Countries
{
    public class CreateCountryValidator : AbstractValidator<CreateCountryDto>
    {
        public CreateCountryValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["CountryNameRequired"].Value)
                .MaximumLength(100).WithMessage(localizer["CountryNameMaxLength"].Value);

            RuleFor(x => x.IsoCode)
                .NotEmpty().WithMessage(localizer["IsoCodeRequired"].Value)
                .Length(2).WithMessage(localizer["IsoCodeLength"].Value);

            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage(localizer["CurrencyCodeRequired"].Value)
                .Length(3).WithMessage(localizer["CurrencyCodeLength"].Value);

            RuleFor(x => x.PhoneCode)
                .NotEmpty().WithMessage(localizer["PhoneCodeRequired"].Value)
                .MaximumLength(6).WithMessage(localizer["PhoneCodeMaxLength"].Value);

            RuleFor(x => x.TimeZoneId)
                .NotEmpty().WithMessage(localizer["TimeZoneRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["TimeZoneMaxLength"].Value);

            RuleFor(x => x.LanguageCode)
                .NotEmpty().WithMessage(localizer["LanguageCodeRequired"].Value)
                .MaximumLength(5).WithMessage(localizer["LanguageCodeMaxLength"].Value);
        }
    }
}
