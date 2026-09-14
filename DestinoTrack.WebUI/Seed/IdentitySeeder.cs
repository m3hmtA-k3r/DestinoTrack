using DestinoTrack.Business.Consts;
using DestinoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace DestinoTrack.WebUI.Seed
{
    // Uygulama açılışında çalışır: rolleri ve ilk Admin'i oluşturur.
    // Kkaç kez çalışırsa çalışsın çift kayıt oluşmaz.
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            // Program.cs'te istek yok; scoped servisler (UserManager, DbContext) için kendi kapsamımızı açıyoruz
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

            // 1 · Roller — RoleManager, NormalizedName'i kendisi doldurur 
            foreach (var roleName in RoleNames.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }

            // 2 · İlk Admin — bilgiler User Secrets'tan okunur, koda ve Git'e girmez
            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("AdminSeed:Email veya AdminSeed:Password bulunamadı — ilk Admin oluşturulmadı.");
                return;
            }

            if (await userManager.FindByEmailAsync(email) != null)
            {
                return;
            }

            var admin = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = configuration["AdminSeed:FirstName"] ?? "Admin",
                LastName = configuration["AdminSeed:LastName"] ?? "DestinoTrack"
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                // Şifre K4 kuralına uymuyorsa sessizce geçilmez — uygulama açılışta nedenini söyler
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"İlk Admin oluşturulamadı: {errors}");
            }

            await userManager.AddToRoleAsync(admin, RoleNames.Admin);
        }
    }
}
