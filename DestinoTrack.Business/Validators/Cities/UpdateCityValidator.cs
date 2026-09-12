using DestinoTrack.DTO.DTOs.CityDtos;
using FluentValidation;

namespace DestinoTrack.Business.Validators.Cities
{
    public class UpdateCityValidator : AbstractValidator<UpdateCityDto>
    {
        public UpdateCityValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Güncellenecek kayıt belirtilmedi.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Şehir adı boş olamaz.")
                .MinimumLength(2).WithMessage("Şehir adı en az 2 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Şehir adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Ülke seçilmelidir.");
        }
    }
}
