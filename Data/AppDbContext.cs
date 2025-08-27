using Microsoft.EntityFrameworkCore;
using GeziRotasi.API.Models; // senin entity’lerin buradaysa

namespace GeziRotasi.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Örnek: Entity tablolarını ekle
        public DbSet<Poi> Pois { get; set; }
        // ileride: Users, Preferences vs eklenebilir

        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<SpecialDayHour> SpecialDayHours { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bu metot, EF Core'un tabloları ve ilişkileri nasıl kuracağını tanımlar.
            // Şimdi ona, tabloyu kurduktan sonra içine hangi verileri ekeceğini söylüyoruz.
            modelBuilder.Entity<Poi>()
               .Property(p => p.Category)
               .HasConversion<string>(); // Category enum'ını veritabanında string olarak sakla.

            modelBuilder.Entity<Poi>().HasData(
                // Buraya, daha önce PoiService'te kullandığımız sahte veri listesini ekliyoruz.
                // ID'leri elle vermemiz çok önemli.
                new Poi { Id = 1, Name = "Topkapı Sarayı", Description = "Osmanlı İmparatorluğu'nun...", Latitude = 41.0116, Longitude = 28.9834, Category = PoiCategory.Müze },
                new Poi { Id = 2, Name = "Kapalıçarşı", Description = "Dünyanın en eski ve en büyük...", Latitude = 41.0107, Longitude = 28.9681, Category = PoiCategory.Alışveriş },
                new Poi { Id = 3, Name = "Sultanahmet Camii", Description = "Mavi Camii olarak da bilinir...", Latitude = 41.0054, Longitude = 28.9768, Category = PoiCategory.Tarihi_Yapı },
                new Poi { Id = 4, Name = "Ayasofya Tarih ve Deneyim Müzesi", Description = "Tarihi yarımadanın kalbinde...", Latitude = 41.0086, Longitude = 28.9800, Category = PoiCategory.Müze },
                new Poi { Id = 5, Name = "Yerebatan Sarnıcı", Description = "Büyüleyici atmosferi ile...", Latitude = 41.0084, Longitude = 28.9779, Category = PoiCategory.Müze }
                // İstersen daha fazla veri ekleyebilirsin
            );
        }
    }
}
