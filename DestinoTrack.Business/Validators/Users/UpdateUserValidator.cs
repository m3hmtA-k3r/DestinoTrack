using DestinoTrack.Business.Consts;
using DestinoTrack.Business.Validators.Common;
using DestinoTrack.DTO.DTOs.UserDtos;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DestinoTrack.Business.Validators.Users
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator(IStringLocalizer<SharedResource> localizer, IOptions<IdentityOptions> identityOptions)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(localizer["RecordIdRequired"].Value);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer["FirstNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["FirstNameMaxLength"].Value);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer["LastNameRequired"].Value)
                .MaximumLength(50).WithMessage(localizer["LastNameMaxLength"].Value);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
                .EmailAddress().WithMessage(localizer["EmailInvalid"].Value)
                .MaximumLength(256).WithMessage(localizer["EmailMaxLength"].Value);

            RuleFor(x => x.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer["RoleRequired"].Value)
                .Must(role => RoleNames.Staff.Contains(role)).WithMessage(localizer["RoleInvalid"].Value);

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(localizer["CountryRequiredForManager"].Value)
                .When(x => x.Role == RoleNames.Manager);

            //iki şifre alanı da boşsa şifre kuralları hiç çalışmaz
            When(x => !string.IsNullOrEmpty(x.NewPassword) || !string.IsNullOrEmpty(x.ConfirmNewPassword), () =>
            {
                RuleFor(x => x.NewPassword)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(localizer["PasswordRequired"].Value)
                    .PasswordPolicy(identityOptions.Value.Password, localizer);

                RuleFor(x => x.ConfirmNewPassword)
                    .Equal(x => x.NewPassword).WithMessage(localizer["ConfirmPasswordMismatch"].Value);
            });
        }
    }
}
