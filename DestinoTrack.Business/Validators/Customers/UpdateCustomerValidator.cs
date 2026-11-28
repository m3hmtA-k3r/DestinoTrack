using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Customers
{
    // Create ile aynı kurallar + kimlik. Kod ve durum formda salt okunur olduğu için
    // burada kuralları yok: UpdateAsync ikisini de yazmıyor (D114 · D115)
    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
    {
        private const string PhonePattern = @"^(?=.{7,20}$)\+?[0-9 ]+$";

        public UpdateCustomerValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(localizer["RecordIdRequired"].Value);

            RuleFor(x => x.CustomerType)
                .IsInEnum().WithMessage(localizer["CustomerTypeInvalid"].Value);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequired"].Value);

            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["CustomerTitleRequired"].Value)
                .MaximumLength(200).WithMessage(localizer["CustomerTitleMaxLength"].Value);

            RuleFor(x => x.TaxNumber)
                .MaximumLength(20).WithMessage(localizer["TaxNumberMaxLength"].Value);

            When(x => x.CustomerType == CustomerType.Individual, () =>
            {
                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage(localizer["IdentityNumberRequired"].Value);
            });

            When(x => x.CustomerType == CustomerType.Corporate, () =>
            {
                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage(localizer["TaxNumberRequired"].Value);

                RuleFor(x => x.TaxOffice)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["TaxOfficeRequired"].Value)
                    .MaximumLength(100).WithMessage(localizer["TaxOfficeMaxLength"].Value);
            });

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
                .EmailAddress().WithMessage(localizer["EmailInvalid"].Value)
                .MaximumLength(100).WithMessage(localizer["CustomerEmailMaxLength"].Value);

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["PhoneRequired"].Value)
                .Matches(PhonePattern).WithMessage(localizer["PhoneInvalid"].Value);
        }
    }
}
