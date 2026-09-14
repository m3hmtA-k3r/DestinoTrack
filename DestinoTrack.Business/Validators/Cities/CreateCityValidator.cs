using DestinoTrack.DTO.DTOs.CityDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Cities
{
    public class CreateCityValidator : AbstractValidator<CreateCityDto>
    {
        // Mesajlar SharedResource'tan gelir. 
        public CreateCityValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["CityNameRequired"].Value)
                .MinimumLength(2).WithMessage(localizer["CityNameMinLength"].Value)
                .MaximumLength(100).WithMessage(localizer["CityNameMaxLength"].Value);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequired"].Value);
        }
    }
}
