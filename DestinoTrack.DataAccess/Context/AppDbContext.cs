using DestinoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Payment> Payments { get; set; }

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

            //Cargo => Kurye
            builder.Entity<Cargo>()
                .HasOne(c => c.Courier)
                .WithMany(k => k.Cargos)
                .HasForeignKey(c => c.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            //CargoMovement
            builder.Entity<CargoMovement>()
                .HasOne(m => m.Cargo)
                .WithMany(c => c.Movements)
                .HasForeignKey(m => m.CargoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CargoMovement>()
                .HasOne(m => m.Branch)
                .WithMany()
                .HasForeignKey(m => m.BranchId)
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

            // Kurye => Şube
            builder.Entity<Courier>()
                .HasOne(k => k.Branch)
                .WithMany(b => b.Couriers)
                .HasForeignKey(k => k.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Şube => Şehir
            builder.Entity<Branch>()
                .HasOne(b => b.City)
                .WithMany(c => c.Branches)
                .HasForeignKey(b => b.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Address => AppUser
            builder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //TrackCode sütununda aynı değer iki kez olamaz
            builder.Entity<Cargo>()
                .HasIndex(c => c.TrackCode)
                .IsUnique();

            // --- Para alanları: kuruş hassasiyeti ---
            builder.Entity<Cargo>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // --- Metin uzunlukları ---
            builder.Entity<Cargo>()
                .Property(c => c.TrackCode).HasMaxLength(30);

            builder.Entity<City>()
                .Property(c => c.Name).HasMaxLength(100);

            builder.Entity<Branch>()
                .Property(b => b.Name).HasMaxLength(100);

            builder.Entity<Courier>(e =>
            {
                e.Property(k => k.FirstName).HasMaxLength(50);
                e.Property(k => k.LastName).HasMaxLength(50);
                e.Property(k => k.PhoneNumber).HasMaxLength(20);
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
        }
    }
}
