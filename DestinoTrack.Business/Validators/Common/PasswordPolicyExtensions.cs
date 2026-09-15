using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Validators.Common
{
    // CreateUser'da Password, UpdateUser'da NewPassword, kayıtta Password aynı kuralı kullanır.
    public static class PasswordPolicyExtensions
    {
        private static readonly Regex Upper = new("[A-Z]");
        private static readonly Regex Lower = new("[a-z]");
        private static readonly Regex Digit = new("[0-9]");
        private static readonly Regex NonAlphanumeric = new("[^a-zA-Z0-9]");

        public static IRuleBuilderOptions<T, string?> PasswordPolicy<T>(this IRuleBuilder<T, string?> rule,
                                                                       PasswordOptions options,
                                                                       IStringLocalizer<SharedResource> localizer)
        {
            // Uzunluk tarayıcıda da denetlenir (data-val-minlength)
            var builder = rule
                .MinimumLength(options.RequiredLength)
                .WithMessage(localizer["Identity_PasswordTooShort", options.RequiredLength].Value);

            // Karakter kuralları Matches DEĞİL Must ile Matches tarayıcıya data-val-regex yazar,
            // jQuery o deseni değerin TAMAMIYLA eşleştirir ("[A-Z]" yalnızca tek harfi kabul eder) ve bir kutuda
            // tek regex tutabildiği için diğer kurallar düşer. Must yalnızca sunucuda çalışır, mesajlar aynı kalır.
            if (options.RequireUppercase)
                builder = builder.Must(p => p != null && Upper.IsMatch(p)).WithMessage(localizer["Identity_PasswordRequiresUpper"].Value);

            if (options.RequireLowercase)
                builder = builder.Must(p => p != null && Lower.IsMatch(p)).WithMessage(localizer["Identity_PasswordRequiresLower"].Value);

            if (options.RequireDigit)
                builder = builder.Must(p => p != null && Digit.IsMatch(p)).WithMessage(localizer["Identity_PasswordRequiresDigit"].Value);

            if (options.RequireNonAlphanumeric)
                builder = builder.Must(p => p != null && NonAlphanumeric.IsMatch(p)).WithMessage(localizer["Identity_PasswordRequiresNonAlphanumeric"].Value);

            return builder;
        }
    }
}
