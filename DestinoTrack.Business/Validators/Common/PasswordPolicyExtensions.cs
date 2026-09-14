using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Common
{    
    public static class PasswordPolicyExtensions
    {// şifre kuralı tek yerde: değerler Program.cs'teki PasswordOptions'tan okunur.
     // CreateUser'da Password, UpdateUser'da NewPassword aynı kuralı kullanır.
        public static IRuleBuilderOptions<T, string?> PasswordPolicy<T>(this IRuleBuilder<T, string?> rule,
                                                                       PasswordOptions options,
                                                                       IStringLocalizer<SharedResource> localizer)
        {
            // Mesajlar Identity'ninkiyle aynı anahtarlardan — form ve Identity aynı cümleyi söyler
            var builder = rule
                .MinimumLength(options.RequiredLength)
                .WithMessage(localizer["Identity_PasswordTooShort", options.RequiredLength].Value);

            if (options.RequireUppercase)
                builder = builder.Matches("[A-Z]").WithMessage(localizer["Identity_PasswordRequiresUpper"].Value);

            if (options.RequireLowercase)
                builder = builder.Matches("[a-z]").WithMessage(localizer["Identity_PasswordRequiresLower"].Value);

            if (options.RequireDigit)
                builder = builder.Matches("[0-9]").WithMessage(localizer["Identity_PasswordRequiresDigit"].Value);

            if (options.RequireNonAlphanumeric)
                builder = builder.Matches("[^a-zA-Z0-9]").WithMessage(localizer["Identity_PasswordRequiresNonAlphanumeric"].Value);

            return builder;
        }
    }
}
