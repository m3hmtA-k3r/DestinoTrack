using DestinoTrack.Business;
using DestinoTrack.Business.Localization;
using DestinoTrack.Business.Services.Cities;
using DestinoTrack.Business.Services.Countries;
using DestinoTrack.Business.Services.Dashboard;
using DestinoTrack.Business.Services.Accounts;
using DestinoTrack.Business.Services.Users;
using DestinoTrack.DataAccess.Interceptors;
using DestinoTrack.WebUI.Infrastructure;
using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.Abouts;
using DestinoTrack.DataAccess.Repositories.Addresses;
using DestinoTrack.DataAccess.Repositories.Branches;
using DestinoTrack.DataAccess.Repositories.CargoMovements;
using DestinoTrack.DataAccess.Repositories.Cargos;
using DestinoTrack.DataAccess.Repositories.Cities;
using DestinoTrack.DataAccess.Repositories.ContactInfos;
using DestinoTrack.DataAccess.Repositories.Countries;
using DestinoTrack.DataAccess.Repositories.Couriers;
using DestinoTrack.DataAccess.Repositories.Customers;
using DestinoTrack.DataAccess.Repositories.Payments;
using DestinoTrack.Entity.Entities;
using DestinoTrack.WebUI.Seed;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Denetimi kaydını "kim yaptı" bilgisi o anki HTTP isteğinden okunur hale getirdik
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, HttpCurrentUserAccessor>();
builder.Services.AddScoped<AuditLogInterceptor>();

// DB bağlantısını yapıyoruz.
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();

    // Sıra önemli — önce tarihler + soft delete, sonra denetim: AuditLog silmeyi "IsDeleted false → true" olarak görür
    options.AddInterceptors(new BaseEntityInterceptor(), sp.GetRequiredService<AuditLogInterceptor>());
});


// Identity için gerekli servisleri ekliyoruz.
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    //  Şifre: en az 8 karakter, büyük + küçük harf + rakam; özel karakter zorunlu değil
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;

    // 5 hatalı denemede 15 dakika kilit — yeni açılan hesaplar için de geçerli
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    // Aynı e-postayla ikinci hesap açılamaz
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

// Oturum çerezi — giriş, çıkış ve erişim engeli adresleri + süre
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // Varsayılan ".AspNetCore.Identity.Application" yerine projeye özel ad
    options.Cookie.Name = "DestinoTrack.Auth";
    // JavaScript çereze erişemez — XSS ile oturum çalınamaz
    options.Cookie.HttpOnly = true;

    // 30 dakika işlem yapılmazsa oturum düşer; her istekte süre yenilenir
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// FluentValidation için gerekli servisler 
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(BusinessAssembly).Assembly);

// --- Çok dillilik 
var supportedCultures = new[] { "tr", "en", "pt" };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("tr")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// Çeviri dosyalarının Resources klasöründe olduğunu söylüyoruz
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Repository Dependency Injection kısmına giriş.
builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<ICargoMovementRepository, CargoMovementRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Servisler
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// Roller ve ilk Admin kayıt varsa dokunmaz
await IdentitySeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Dil seçimi çerezden okunur — Authentication'dan önce çalışmalı
app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
