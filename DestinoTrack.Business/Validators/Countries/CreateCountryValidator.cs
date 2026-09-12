using DestinoTrack.DTO.DTOs.CountryDtos;
using FluentValidation;

namespace DestinoTrack.Business.Validators.Countries
{
    public class CreateCountryValidator : AbstractValidator<CreateCountryDto>
    {
        public CreateCountryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ülke adı boş olamaz.")
                .MaximumLength(100).WithMessage("Ülke adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.IsoCode)
                .NotEmpty().WithMessage("ISO kodu boş olamaz.")
                .Length(2).WithMessage("ISO kodu 2 harf olmalıdır. Örnek: TR, MT, BR");

            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage("Para birimi kodu boş olamaz.")
                .Length(3).WithMessage("Para birimi kodu 3 harf olmalıdır. Örnek: TRY, EUR, BRL");

            RuleFor(x => x.PhoneCode)
                .NotEmpty().WithMessage("Telefon kodu boş olamaz.")
                .MaximumLength(6).WithMessage("Telefon kodu en fazla 6 karakter olabilir.");

            RuleFor(x => x.TimeZoneId)
                .NotEmpty().WithMessage("Saat dilimi boş olamaz.")
                .MaximumLength(50).WithMessage("Saat dilimi en fazla 50 karakter olabilir.");

            RuleFor(x => x.LanguageCode)
                .NotEmpty().WithMessage("Dil kodu boş olamaz.")
                .MaximumLength(5).WithMessage("Dil kodu en fazla 5 karakter olabilir.");
        }
    }
}
