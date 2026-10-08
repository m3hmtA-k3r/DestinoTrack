using System.Security.Claims;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.WebUI.Infrastructure
{
    // ICurrentUserAccessor'ın web karşılığı: o anki isteğin oturum çerezindeki kullanıcı
    //
    // UserManager KURUCUDA İSTENMEZ: AppDbContext kurulurken AuditLogInterceptor → bu sınıf → UserManager
    // =>  UserStore => AppDbContext döngüsü oluşur ve uygulama açılışta kilitlenir.
    // Bunun yerine ihtiyaç anında çözülür; o anda DbContext zaten hazır olduğu için döngü kapanmaz.
    public class HttpCurrentUserAccessor(IHttpContextAccessor _httpContextAccessor,
                                         IServiceProvider _serviceProvider) : ICurrentUserAccessor
    {
        //şube/ülke veritabanından okunur. İstek başına bir kez okunup burada tutulur
        private AppUser? _user;
        private bool _loaded;

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        // Girişsiz istekte ya da istek dışında (uygulama açılışı) null
        public Guid? UserId =>
            Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        // Identity e-postayı ayrı claim olarak da yazar; yoksa kullanıcı adı (bizim kullanıcı adımız = e-posta)
        public string? Email => User?.FindFirstValue(ClaimTypes.Email) ?? User?.Identity?.Name;

        // Rol çerezde duruyor, veritabanına gitmeye gerek yok
        public string? Role => User?.FindFirstValue(ClaimTypes.Role);

        public Guid? BranchId => Load()?.BranchId;
        public Guid? CountryId => Load()?.CountryId;
        public Guid? CustomerId => Load()?.CustomerId;

        //claim'e yazsaydık şube değişince çerez eskir; her istekte okuyup güncel tutuyoruz.
        // Aynı istekte ikinci kez sorulursa veritabanına tekrar gidilmez
        private AppUser? Load()
        {
            if (_loaded)
            {
                return _user;
            }

            _loaded = true;
            var id = UserId;
            if (id.HasValue)
            {
                // UserManager burada çözülür: AppDbContext bu noktada kurulmuş durumda
                var userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();
                _user = userManager.Users.FirstOrDefault(u => u.Id == id.Value);
            }

            return _user;
        }
    }
}
