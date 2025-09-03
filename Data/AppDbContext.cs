using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GeziRotasi.API.Entities;
using GeziRotasi.API.Domain.Categories;

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
        public DbSet<Category> Categories { get; set; }

        // -------------------
        // PUANLAMA & KULLANICI TERCİHLERİ
        // -------------------
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<RouteFeedback> RouteFeedbacks { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<TravelRoute> TravelRoutes { get; set; }


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

            // ---- TravelRoute PK Fix ----
            modelBuilder.Entity<TravelRoute>(e =>
            {
                e.ToTable("TravelRoutes");
                e.HasKey(x => x.Id).HasName("PK_TravelRoutes"); // Özel PK adı

                // ---- Category Seed ----
                modelBuilder.Entity<Category>().HasData(
                    new Category { Id = 1, Name = "Alışveriş", Path = "/images/categories/alisveris.png", Depth = 0 },
                    new Category { Id = 2, Name = "Eğlence", Path = "/images/categories/eglence.png", Depth = 0 },
                    new Category { Id = 3, Name = "Karayolu", Path = "/images/categories/karayolu.png", Depth = 0 },
                    new Category { Id = 4, Name = "Kültür", Path = "/images/categories/kultur.png", Depth = 0 },
                    new Category { Id = 5, Name = "KültürelTesisler", Path = "/images/categories/kulturel.png", Depth = 0 },
                    new Category { Id = 6, Name = "OnemliNoktalar", Path = "/images/categories/onemli.png", Depth = 0 },
                    new Category { Id = 7, Name = "TarihiTuristikTesisler", Path = "/images/categories/tarihi.png", Depth = 0 },
                    new Category { Id = 8, Name = "Yemek", Path = "/images/categories/yemek.png", Depth = 0 }
                );
            });
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
