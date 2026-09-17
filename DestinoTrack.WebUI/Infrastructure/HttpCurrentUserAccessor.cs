using System.Security.Claims;
using DestinoTrack.DataAccess.Interceptors;

namespace DestinoTrack.WebUI.Infrastructure
{
    // ICurrentUserAccessor'ın web karşılığı: o anki isteğin oturum çerezindeki kullanıcı
    public class HttpCurrentUserAccessor(IHttpContextAccessor _httpContextAccessor) : ICurrentUserAccessor
    {
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        // Girişsiz istekte ya da istek dışında (uygulama açılışı) null
        public Guid? UserId =>
            Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        // Identity e-postayı ayrı claim olarak da yazar; yoksa kullanıcı adı (bizim kullanıcı adımız = e-posta)
        public string? Email => User?.FindFirstValue(ClaimTypes.Email) ?? User?.Identity?.Name;
    }
}
