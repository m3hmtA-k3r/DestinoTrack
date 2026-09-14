using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DestinoTrack.Business.Localization
{
    // Identity'nin İngilizce hata mesajlarını SharedResource'tan seçili dilde üretir.
    // Code değerleri Identity'ninkiyle aynı kalır — ileride hatayı forma alanına eşlemek için kullanılır.
    public class LocalizedIdentityErrorDescriber(IStringLocalizer<SharedResource> _localizer) : IdentityErrorDescriber
    {
        // Resx anahtarı: "Identity_" + Code → ör. Identity_PasswordTooShort
        private IdentityError Error(string code, params object[] arguments) => new()
        {
            Code = code,
            Description = _localizer["Identity_" + code, arguments].Value
        };

        // Genel
        public override IdentityError DefaultError() => Error(nameof(DefaultError));
        public override IdentityError ConcurrencyFailure() => Error(nameof(ConcurrencyFailure));
        public override IdentityError InvalidToken() => Error(nameof(InvalidToken));
        public override IdentityError RecoveryCodeRedemptionFailed() => Error(nameof(RecoveryCodeRedemptionFailed));
        public override IdentityError LoginAlreadyAssociated() => Error(nameof(LoginAlreadyAssociated));

        // Kullanıcı adı ve e-posta
        public override IdentityError InvalidUserName(string? userName) => Error(nameof(InvalidUserName), userName ?? string.Empty);
        public override IdentityError InvalidEmail(string? email) => Error(nameof(InvalidEmail), email ?? string.Empty);
        public override IdentityError DuplicateUserName(string userName) => Error(nameof(DuplicateUserName), userName);
        public override IdentityError DuplicateEmail(string email) => Error(nameof(DuplicateEmail), email);

        // Roller
        public override IdentityError InvalidRoleName(string? role) => Error(nameof(InvalidRoleName), role ?? string.Empty);
        public override IdentityError DuplicateRoleName(string role) => Error(nameof(DuplicateRoleName), role);
        public override IdentityError UserAlreadyInRole(string role) => Error(nameof(UserAlreadyInRole), role);
        public override IdentityError UserNotInRole(string role) => Error(nameof(UserNotInRole), role);

        // Hesap
        public override IdentityError UserAlreadyHasPassword() => Error(nameof(UserAlreadyHasPassword));
        public override IdentityError UserLockoutNotEnabled() => Error(nameof(UserLockoutNotEnabled));

        // Şifre kuralları 
        public override IdentityError PasswordMismatch() => Error(nameof(PasswordMismatch));
        public override IdentityError PasswordTooShort(int length) => Error(nameof(PasswordTooShort), length);
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => Error(nameof(PasswordRequiresUniqueChars), uniqueChars);
        public override IdentityError PasswordRequiresNonAlphanumeric() => Error(nameof(PasswordRequiresNonAlphanumeric));
        public override IdentityError PasswordRequiresDigit() => Error(nameof(PasswordRequiresDigit));
        public override IdentityError PasswordRequiresLower() => Error(nameof(PasswordRequiresLower));
        public override IdentityError PasswordRequiresUpper() => Error(nameof(PasswordRequiresUpper));
    }
}
