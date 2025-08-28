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
        }
    }
}
