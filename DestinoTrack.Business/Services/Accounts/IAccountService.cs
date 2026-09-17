using System.Security.Claims;
using DestinoTrack.DTO.DTOs.AccountDtos;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.Business.Services.Accounts // Giriş · çıkış · kayıt · profile — AccountController'daki Identity işleri burada  
{
    public interface IAccountService
    {
        // Şifreyle giriş
        Task<LoginResultDto> LoginAsync(LoginDto loginDto);

        Task LogoutAsync();

        //Customer + AppUser + Customer rolü; başarılıysa kullanıcı giriş yapmış olur
        Task<IdentityResult> RegisterCustomerAsync(RegisterDto registerDto);

        // Giriş yapan kullanıcının Hesabım bilgisi; kullanıcı artık yoksa null
        Task<ProfileDto?> GetProfileAsync(ClaimsPrincipal principal);

        // Layout'taki kullanıcı kartı ve üst menü (ViewComponent'ler); girişsizse ya da kullanıcı artık yoksa null
        Task<UserCardDto?> GetUserCardAsync(ClaimsPrincipal principal);
    }
}
