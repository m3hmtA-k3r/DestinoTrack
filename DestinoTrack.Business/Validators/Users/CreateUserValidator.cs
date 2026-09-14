using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Validators.Common;
using DestinoTrack.DTO.DTOs.UserDtos;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DestinoTrack.Business.Validators.Users
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        // IdentityOptions:Değerleri Program.cs'ten okunur — kural iki yerde yazılmaz
        public CreateUserValidator(IStringLocalizer<SharedResource> localizer, IOptions<IdentityOptions> identityOptions)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer["FirstNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"].Value);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer["LastNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"].Value);

            // Stop: ilk hatada durur — boş alanda arka arkaya birkaç mesaj çıkmaz
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
                .EmailAddress().WithMessage(localizer["EmailInvalid"].Value)
                .MaximumLength(256).WithMessage(localizer["EmailMaxLength"].Value);

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["PasswordRequired"].Value)
                .PasswordPolicy(identityOptions.Value.Password, localizer);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage(localizer["ConfirmPasswordMismatch"].Value);

            // Customer bu ekrandan açılamaz yalnızca personel rolleri
            RuleFor(x => x.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["RoleRequired"].Value)
                .Must(role => RoleNames.Staff.Contains(role)).WithMessage(localizer["RoleInvalid"].Value);

            // Manager bir ülkeden sorumludur
            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequiredForManager"].Value)
                .When(x => x.Role == RoleNames.Manager);
        }
    }
}
