using GeziRotasi.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GeziRotasi.API.Entities; // EmailCode, PasswordHistory

namespace GeziRotasi.API.Data
{
    // IdentityDbContext -> AppUser ile tüm kullanıcı işlemleri + kendi tablolarımız
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // -------------------
        // AUTH / IDENTITY TABLOLARI
        // -------------------
        public DbSet<EmailCode> EmailCodes => Set<EmailCode>();
        public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();


        // -------------------
        // POI & ROTA TABLOLARI
        // -------------------
        public DbSet<Poi> Pois { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<SpecialDayHour> SpecialDayHours { get; set; }

        // -------------------
        // PUANLAMA & KULLANICI TERCİHLERİ
        // -------------------
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RouteFeedback> RouteFeedbacks { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }

        // -------------------
        // MODEL YAPILANDIRMA
        // -------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- EmailCode Mapping ----
            modelBuilder.Entity<EmailCode>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.UserId, x.Purpose, x.CreatedAtUtc });
                e.HasOne(x => x.User).WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ---- PasswordHistory Mapping ----
            modelBuilder.Entity<PasswordHistory>(e =>
            {
                e.ToTable("PasswordHistories");
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                e.Property(x => x.PasswordHash).IsRequired();

                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ---- POI Category Enum Mapping ----
            modelBuilder.Entity<Poi>()
                .Property(p => p.Category)
                .HasConversion<string>();

            // ---- Seed Data for POIs ----
            modelBuilder.Entity<Poi>().HasData(
                new Poi { Id = 1, Name = "Topkapı Sarayı", Description = "Osmanlı İmparatorluğu'nun...", Latitude = 41.0116, Longitude = 28.9834, Category = PoiCategory.Müze },
                new Poi { Id = 2, Name = "Kapalıçarşı", Description = "Dünyanın en eski ve en büyük...", Latitude = 41.0107, Longitude = 28.9681, Category = PoiCategory.Alışveriş },
                new Poi { Id = 3, Name = "Sultanahmet Camii", Description = "Mavi Camii olarak da bilinir...", Latitude = 41.0054, Longitude = 28.9768, Category = PoiCategory.Tarih },
                new Poi { Id = 4, Name = "Ayasofya Tarih ve Deneyim Müzesi", Description = "Tarihi yarımadanın kalbinde...", Latitude = 41.0086, Longitude = 28.9800, Category = PoiCategory.Müze },
                new Poi { Id = 5, Name = "Yerebatan Sarnıcı", Description = "Büyüleyici atmosferi ile...", Latitude = 41.0084, Longitude = 28.9779, Category = PoiCategory.Müze }
            );
        }
    }
}
