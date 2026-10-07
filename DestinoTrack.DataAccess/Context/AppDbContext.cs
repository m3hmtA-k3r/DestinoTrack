using DestinoTrack.Entity.Entities;
using DestinoTrack.Entity.Entities.Common;
using DestinoTrack.Entity.Entities.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DestinoTrack.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CargoMovement> CargoMovements { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ContactInfo> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CargoPrice> CargoPrices { get; set; }
        public DbSet<CargoTypeRate> CargoTypeRates { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryException> DeliveryExceptions { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); //ikili ilişkileri kuruyoruz



            /// Cargo ==> AppUser
            builder.Entity<Cargo>()
                .HasOne(x => x.Sender)
                .WithMany(u => u.SentCargos)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cargo>()
                .HasOne(x => x.Receiver)
                .WithMany(u => u.ReceivedCargos)
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            //Cargo => Branch
            builder.Entity<Cargo>()
                .HasOne(x => x.OriginBranch)
                .WithMany(b => b.OriginCargos)
                .HasForeignKey(c => c.OriginBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cargo>()
                .HasOne(x => x.DestinationBranch)
                .WithMany(b => b.DestinationCargos)
                .HasForeignKey(c => c.DestinationBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cargo => Kurye (Employee, JobType = Courier)
            builder.Entity<Cargo>()
                .HasOne(c => c.Courier)
                .WithMany(k => k.Cargos)
                .HasForeignKey(c => c.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Teslimat : kargo başına tek başarılı teslim — bire bir ilişki
            builder.Entity<Delivery>()
                .HasOne(d => d.Cargo)
                .WithOne(c => c.Delivery)
                .HasForeignKey<Delivery>(d => d.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Teslimi yapan personel: kayıt silinmez
            builder.Entity<Delivery>()
                .HasOne(d => d.DeliveredByEmployee)
                .WithMany()
                .HasForeignKey(d => d.DeliveredByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Başarısız denemeler : kargo başına birden çok satır
            builder.Entity<DeliveryException>()
                .HasOne(x => x.Cargo)
                .WithMany(c => c.DeliveryExceptions)
                .HasForeignKey(x => x.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DeliveryException>()
                .HasOne(x => x.AttemptedByEmployee)
                .WithMany()
                .HasForeignKey(x => x.AttemptedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            //CargoMovement
            // Hareket geçmişi silinmez 
            builder.Entity<CargoMovement>()
                .HasOne(m => m.Cargo)
                .WithMany(c => c.Movements)
                .HasForeignKey(m => m.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CargoMovement>()
                .HasOne(m => m.Branch)
                .WithMany()
                .HasForeignKey(m => m.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // İşlemi yapan kullanıcı => kullanıcıya bağlı hareketler, kullanıcıyla birlikte silinmez
            builder.Entity<CargoMovement>()
                .HasOne(m => m.PerformedByUser)
                .WithMany()
                .HasForeignKey(m => m.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            //Payment => Cargo / Kurye / Şube
            builder.Entity<Payment>()
                .HasOne(p => p.Cargo)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.CollectedByCourier)
                .WithMany(k => k.CollectedPayments)
                .HasForeignKey(p => p.CollectedByCourierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.CollectedByBranch)
                .WithMany(b => b.CollectedPayments)
                .HasForeignKey(p => p.CollectedByBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Personel => Şube
            builder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Personel => kullanıcı hesabı (varsa) : giriş yapmayan personelde boş kalır
            // WithMany() boş: AppUser tarafında personel listesi tutulmuyor
            builder.Entity<Employee>()
                .HasOne(e => e.AppUser)
                .WithMany()
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tarife => çıkış ülkesi: para birimi bu ülkeden okunur
            // WithMany() boş: Country tarafında tarife listesi tutulmuyor
            builder.Entity<CargoPrice>()
                .HasOne(p => p.Country)
                .WithMany()
                .HasForeignKey(p => p.CountryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Şube => Şehir
            builder.Entity<Branch>()
                .HasOne(b => b.City)
                .WithMany(c => c.Branches)
                .HasForeignKey(b => b.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Şehir => Ülke
            builder.Entity<City>()
                .HasOne(c => c.Country)
                .WithMany(k => k.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Address => AppUser
            builder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //müşteri => Country
            builder.Entity<Customer>()
                .HasOne(m => m.Country)
                .WithMany()
                .HasForeignKey(m => m.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kargo => Müşteri (kurumsal gönderi)
            builder.Entity<Cargo>()
                .HasOne(c => c.Customer)
                .WithMany(m => m.Cargos)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Şube => Yönetici (AppUser)
            builder.Entity<Branch>()
                .HasOne(b => b.Manager)
                .WithMany()
                .HasForeignKey(b => b.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer => Müşteri hesabı (kurumsal kullanıcı)
            builder.Entity<AppUser>()
                .HasOne(u => u.Customer)
                .WithMany(m => m.Users)
                .HasForeignKey(u => u.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kullanıcı kapsamı : Manager → ülke
            builder.Entity<AppUser>()
                .HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kullanıcı kapsamı: Personel / Courier → şube
            // Branch.Manager da AppUser'a bağlı; ikisi ayrı ilişki — WithMany() ikisinde de açıkça yazılı
            builder.Entity<AppUser>()
                .HasOne(u => u.Branch)
                .WithMany()
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);


            // Tekil indeksler yalnızca silinmemiş kayıtlarda geçerli: silinen kayıt aynı değerin yeniden eklenmesini engellemez

            //TrackCode sütununda aynı değer iki kez olamaz
            builder.Entity<Cargo>()
                .HasIndex(c => c.TrackCode)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Aynı ISO kodu iki ülkede olamaz (TR, MT, BR)
            builder.Entity<Country>()
                .HasIndex(c => c.IsoCode)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Aynı ülkede aynı şehir iki kez olamaz  
            // İki sütun birlikte unique: "Ankara" Türkiye'de bir kez, ama başka ülkede aynı ad olabilir
            builder.Entity<City>()
                .HasIndex(c => new { c.CountryId, c.Name })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Aynı barkod iki kargoda olamaz
            builder.Entity<Cargo>()
                .HasIndex(c => c.Barcode)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Aynı tesis kodu iki şubede olamaz ( kod elle girilir, tekildir)
            builder.Entity<Branch>()
                .HasIndex(b => b.Code)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Aynı müşteri kodu iki hesapta olamaz
            builder.Entity<Customer>()
                .HasIndex(m => m.Code)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Bir ülkenin bir kademesi için tek tarife olur
            builder.Entity<CargoPrice>()
                .HasIndex(p => new { p.CountryId, p.RouteScope })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Her kargo tipi için tek çarpan satırı
            builder.Entity<CargoTypeRate>()
                .HasIndex(r => r.CargoType)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Bir kargo bir kez teslim edilir
            builder.Entity<Delivery>()
                .HasIndex(d => d.CargoId)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // Aynı deneme numarası bir kargoda iki kez yazılamaz
            builder.Entity<DeliveryException>()
                .HasIndex(x => new { x.CargoId, x.AttemptNo })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");


            // --- Para alanları: kuruş hassasiyeti ---
            builder.Entity<Cargo>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Employee>()
                .Property(e => e.Rating)
                .HasPrecision(3, 2);          // 4,85

            builder.Entity<Customer>(e =>
            {
                e.Property(m => m.CreditLimit).HasPrecision(18, 2);
                e.Property(m => m.CurrentBalance).HasPrecision(18, 2);
            });

            builder.Entity<CargoPrice>(e =>
            {
                e.Property(p => p.BasePrice).HasPrecision(18, 2);
                e.Property(p => p.PricePerDesi).HasPrecision(18, 2);
            });

            builder.Entity<CargoTypeRate>()
                .Property(r => r.Multiplier)
                .HasPrecision(5, 2);          // 1,50 = %150



            // --- Metin uzunlukları ---
            builder.Entity<Country>(e =>
            {
                e.Property(c => c.Name).HasMaxLength(100);
                e.Property(c => c.IsoCode).HasMaxLength(2);
                e.Property(c => c.CurrencyCode).HasMaxLength(3);
                e.Property(c => c.PhoneCode).HasMaxLength(6);
                e.Property(c => c.TimeZoneId).HasMaxLength(50);
                e.Property(c => c.LanguageCode).HasMaxLength(5);
            });

            // büyük/küçük harf ve aksan duyarsız — "Sao Paulo" = "São Paulo", "ankara" = "Ankara" 
            builder.Entity<City>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .UseCollation("Latin1_General_CI_AI");

            builder.Entity<Branch>(e =>
            {
                e.Property(b => b.Name).HasMaxLength(100);
                e.Property(b => b.Code).HasMaxLength(20);
            });

            builder.Entity<Employee>(e =>
            {
                e.Property(x => x.FirstName).HasMaxLength(50);
                e.Property(x => x.LastName).HasMaxLength(50);
                e.Property(x => x.PhoneNumber).HasMaxLength(20);
                e.Property(x => x.VehiclePlate).HasMaxLength(20);
                e.Property(x => x.Region).HasMaxLength(100);
            });

            // Kargo metinleri tek blokta: takip/barkod · teslimat kodu · gönderici ve alıcı bilgisi 
            builder.Entity<Cargo>(e =>
            {
                e.Property(c => c.TrackCode).HasMaxLength(30);
                e.Property(c => c.CurrencyCode).HasMaxLength(3);
                e.Property(c => c.Barcode).HasMaxLength(50);
                e.Property(c => c.DeliveryCode).HasMaxLength(6);     //  6 hane
                e.Property(c => c.SenderName).HasMaxLength(100);
                e.Property(c => c.SenderPhone).HasMaxLength(20);
                e.Property(c => c.SenderAddress).HasMaxLength(500);
                e.Property(c => c.ReceiverName).HasMaxLength(100);
                e.Property(c => c.ReceiverPhone).HasMaxLength(20);
                e.Property(c => c.ReceiverAddress).HasMaxLength(500);
            });

            builder.Entity<Delivery>(e =>
            {
                e.Property(d => d.Code).HasMaxLength(6);
                e.Property(d => d.RecipientName).HasMaxLength(100);
            });

            builder.Entity<DeliveryException>()
                .Property(x => x.Description).HasMaxLength(500);

            builder.Entity<Customer>(e =>
            {
                e.Property(m => m.Code).HasMaxLength(20);
                e.Property(m => m.Title).HasMaxLength(200);
                e.Property(m => m.TaxNumber).HasMaxLength(20);
                e.Property(m => m.TaxOffice).HasMaxLength(100);
                e.Property(m => m.Email).HasMaxLength(100);
                e.Property(m => m.PhoneNumber).HasMaxLength(20);
            });

            builder.Entity<ContactInfo>(e =>
            {
                e.Property(c => c.Address).HasMaxLength(250);
                e.Property(c => c.Email).HasMaxLength(100);
                e.Property(c => c.PhoneNumber).HasMaxLength(20);
            });

            builder.Entity<Address>(e =>
            {
                e.Property(a => a.Title).HasMaxLength(50);
                e.Property(a => a.City).HasMaxLength(100);
                e.Property(a => a.District).HasMaxLength(100);
                e.Property(a => a.PostalCode).HasMaxLength(20);
                e.Property(a => a.FullAddress).HasMaxLength(500);
            });

            builder.Entity<About>(e =>
            {
                e.Property(a => a.Title).HasMaxLength(200);
                e.Property(a => a.ImageUrl).HasMaxLength(500);
                // Description bilerek MAX kalıyor — uzun metin
            });

            builder.Entity<CargoMovement>()
                .Property(m => m.Description).HasMaxLength(500);

            builder.Entity<Payment>()
                .Property(p => p.ReferenceNo).HasMaxLength(100);

            builder.Entity<AppUser>(e =>
            {
                e.Property(u => u.FirstName).HasMaxLength(50);
                e.Property(u => u.LastName).HasMaxLength(50);
            });

            // --- Denetim kaydı ---
            builder.Entity<AuditLog>(e =>
            {
                e.Property(a => a.EntityName).HasMaxLength(100);
                e.Property(a => a.UserEmail).HasMaxLength(256);   // Identity'deki e-posta uzunluğuyla aynı
                e.Property(a => a.Description).HasMaxLength(500);
                // OldValues / NewValues bilerek MAX: JSON, kaydın bütün alanlarını taşıyabilir

                // "Bu kaydın geçmişi" sorgusu: hangi tablo + hangi kayıt
                e.HasIndex(a => new { a.EntityName, a.EntityId });

                // Denetim ekranı tarihe göre sıralar ve süzer
                e.HasIndex(a => a.CreatedDate);
            });



            // Sabit oluşturma tarihi: yoksa her migration'da seed farkı çıkar
            var seedDate = new DateTime(2026, 9, 7, 0, 0, 0, DateTimeKind.Utc);
            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Türkiye",
                    IsoCode = "TR",
                    CurrencyCode = "TRY",
                    PhoneCode = "+90",
                    TimeZoneId = "Europe/Istanbul",
                    LanguageCode = "tr",
                    CreatedDate = seedDate
                },
                new Country
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Malta",
                    IsoCode = "MT",
                    CurrencyCode = "EUR",
                    PhoneCode = "+356",
                    TimeZoneId = "Europe/Malta",
                    LanguageCode = "en",
                    CreatedDate = seedDate
                },
                new Country
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Brasil",
                    IsoCode = "BR",
                    CurrencyCode = "BRL",
                    PhoneCode = "+55",
                    TimeZoneId = "America/Sao_Paulo",
                    LanguageCode = "pt",
                    CreatedDate = seedDate
                });


            // Kargo tipi çarpanları: sekiz tip, sekiz satır — Admin ekranından değiştirilebilir
            builder.Entity<CargoTypeRate>().HasData(
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000001"), CargoType = CargoType.Standard, Multiplier = 1.00m, TransitDaysDelta = 0, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000002"), CargoType = CargoType.Urgent, Multiplier = 1.50m, TransitDaysDelta = -1, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000003"), CargoType = CargoType.Fragile, Multiplier = 1.30m, TransitDaysDelta = 0, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000004"), CargoType = CargoType.Heavy, Multiplier = 1.40m, TransitDaysDelta = 0, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000005"), CargoType = CargoType.Document, Multiplier = 0.80m, TransitDaysDelta = 0, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000006"), CargoType = CargoType.ColdChain, Multiplier = 1.80m, TransitDaysDelta = 0, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000007"), CargoType = CargoType.Valuable, Multiplier = 1.60m, TransitDaysDelta = 0, CreatedDate = seedDate, IsDeleted = false },
                new CargoTypeRate { Id = Guid.Parse("cc000000-0000-0000-0000-000000000008"), CargoType = CargoType.Oversized, Multiplier = 1.70m, TransitDaysDelta = 1, CreatedDate = seedDate, IsDeleted = false }
            );


            //  Soft delete + tarihler  
            foreach (var entityType in builder.Model.GetEntityTypes().Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)).ToList())
            {
                // Silinmiş kayıt hiçbir sorguda görünmez:  
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var notDeleted = Expression.Lambda(Expression.Not(Expression.Property(parameter, nameof(BaseEntity.IsDeleted))), parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(notDeleted);

                builder.Entity(entityType.ClrType).Property(nameof(BaseEntity.CreatedDate)).HasDefaultValueSql("SYSUTCDATETIME()");
            }
        }



    }
}
