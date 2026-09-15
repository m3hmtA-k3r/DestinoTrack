using DestinoTrack.Business.Validators.Common;
using DestinoTrack.DTO.DTOs.AccountDtos;
using DestinoTrack.Entity.Entities.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DestinoTrack.Business.Validators.Accounts
{
    // Veritabanına gitmeyen kurallar. Kimlik / vergi numarasının ÜLKEYE göre biçimi
    // AccountService'te TaxIdentityRules ile denetlenir — otomatik doğrulama async kural çalıştıramaz.
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator(IStringLocalizer<SharedResource> localizer, IOptions<IdentityOptions> identityOptions)
        {
            RuleFor(x => x.CustomerType)
                .IsInEnum().WithMessage(localizer["CustomerTypeInvalid"].Value);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer["FirstNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"].Value);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer["LastNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"].Value);

            // Customers.Email 100 karakter — AppUser'ın 256 sınırından dar olan geçerli
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
                .EmailAddress().WithMessage(localizer["EmailInvalid"].Value)
                .MaximumLength(100).WithMessage(localizer["CustomerEmailMaxLength"].Value);

            // Rakam, boşluk ve baştaki +; toplam 7–20 karakter (Customers.PhoneNumber 20)
            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["PhoneRequired"].Value)
                .Matches(@"^(?=.{7,20}$)\+?[0-9 ]+$").WithMessage(localizer["PhoneInvalid"].Value);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequired"].Value);

            //Bireyselde kimlik no zorunlu
            When(x => x.CustomerType == CustomerType.Individual, () =>
            {
                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage(localizer["IdentityNumberRequired"].Value);
            });

            // kurumsalda vergi no + şirket ünvanı + vergi dairesi zorunlu
            When(x => x.CustomerType == CustomerType.Corporate, () =>
            {
                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage(localizer["TaxNumberRequired"].Value);

                RuleFor(x => x.CompanyTitle)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["CompanyTitleRequired"].Value)
                    .MaximumLength(200).WithMessage(localizer["CompanyTitleMaxLength"].Value);

                RuleFor(x => x.TaxOffice)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["TaxOfficeRequired"].Value)
                    .MaximumLength(100).WithMessage(localizer["TaxOfficeMaxLength"].Value);
            });

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["PasswordRequired"].Value)
                .PasswordPolicy(identityOptions.Value.Password, localizer);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage(localizer["ConfirmPasswordMismatch"].Value);
        }
    }
}
