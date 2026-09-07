using DestinoTrack.DataAccess.Context;
using DestinoTrack.DataAccess.Repositories.Abouts;
using DestinoTrack.DataAccess.Repositories.Addresses;
using DestinoTrack.DataAccess.Repositories.Branches;
using DestinoTrack.DataAccess.Repositories.CargoMovements;
using DestinoTrack.DataAccess.Repositories.Cargos;
using DestinoTrack.DataAccess.Repositories.Cities;
using DestinoTrack.DataAccess.Repositories.ContactInfos;
using DestinoTrack.DataAccess.Repositories.Couriers;
using DestinoTrack.DataAccess.Repositories.Payments;
using DestinoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//DBBaglantýsýný yapýyoruz.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});

// Identity için gerekli servisleri ekliyoruz.
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

/// REpository Dependency Injection Kýsmýna giriþ. 
builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<ICargoMovementRepository, CargoMovementRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
