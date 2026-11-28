using DestinoTrack.DTO.DTOs.CustomerDtos;
using DestinoTrack.Entity.Entities.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Customers
{
    // Veritabanına gitmeyen kurallar. Vergi numarasının ÜLKEYE göre biçimi ve aynı ülkede
    // tekrar edip etmediği CustomerService'te denetlenir — RegisterValidator'daki ayrımın aynısı
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
    {
        // Kayıt ve kargo formlarıyla aynı telefon kuralı: rakam, boşluk ve baştaki +, 7–20 karakter
        private const string PhonePattern = @"^(?=.{7,20}$)\+?[0-9 ]+$";

        public CreateCustomerValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.CustomerType)
                .IsInEnum().WithMessage(localizer["CustomerTypeInvalid"].Value);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequired"].Value);

            // Kurumsalda ünvan, bireyselde ad soyad — tek alan olduğu için tek kural
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["CustomerTitleRequired"].Value)
                .MaximumLength(200).WithMessage(localizer["CustomerTitleMaxLength"].Value);

            // Uzunluk türden bağımsız, When dışında: iki dalda tekrar yazmayalım
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

            // Customers.Email sütunu 100 karakter
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
