using DestinoTrack.DTO.DTOs.AccountDtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Accounts
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
                .EmailAddress().WithMessage(localizer["EmailInvalid"].Value);

            // Kural kayıt ve şifre belirlemede uygulanır; girişte uygulanırsa
            // saldırgana şifre kuralını ipucu olarak vermiş oluruz.
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer["PasswordRequired"].Value);
        }
    }
}
