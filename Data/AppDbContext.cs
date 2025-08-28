using Microsoft.EntityFrameworkCore;
using GeziRotasi.API.Models; // Tek bir using yeterli.

namespace GeziRotasi.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }

        // --- VERİTABANI TABLOLARI (DbSet) ---

        // POI Modülü Tabloları (Senin ve Sena'nın Sorumluluğu)
        public DbSet<Poi> Pois { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<SpecialDayHour> SpecialDayHours { get; set; }

        // Puanlama Modülü Tabloları (Nisa'nın Sorumluluğu)
        /// <summary>
        /// Mekanlar için yapılan yorum ve puanlamaları tutan tablo.
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// Tamamlanan rotalar için yapılan genel değerlendirmeleri tutan tablo.
        /// </summary>
        public DbSet<RouteFeedback> RouteFeedbacks { get; set; }
        public DbSet<UserPreferences>  UserPreferences { get; internal set; }



        // --- VERİTABANI MODELİ YAPILANDIRMASI ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category enum'ını veritabanında metin (string) olarak sakla.
            // Bu, veritabanını okumayı kolaylaştırır.
            modelBuilder.Entity<Poi>()
                .Property(p => p.Category)
                .HasConversion<string>();

            // Veritabanı ilk oluşturulduğunda eklenecek olan başlangıç verileri (Seed Data).
            modelBuilder.Entity<Poi>().HasData(
                new Poi { Id = 1, Name = "Topkapı Sarayı", Description = "Osmanlı İmparatorluğu'nun...", Latitude = 41.0116, Longitude = 28.9834, Category = PoiCategory.Müze },
                new Poi { Id = 2, Name = "Kapalıçarşı", Description = "Dünyanın en eski ve en büyük...", Latitude = 41.0107, Longitude = 28.9681, Category = PoiCategory.Alışveriş },
                new Poi { Id = 3, Name = "Sultanahmet Camii", Description = "Mavi Camii olarak da bilinir...", Latitude = 41.0054, Longitude = 28.9768, Category = PoiCategory.Tarih },
                new Poi { Id = 4, Name = "Ayasofya Tarih ve Deneyim Müzesi", Description = "Tarihi yarımadanın kalbinde...", Latitude = 41.0086, Longitude = 28.9800, Category = PoiCategory.Müze },
                new Poi { Id = 5, Name = "Yerebatan Sarnıcı", Description = "Büyüleyici atmosferi ile...", Latitude = 41.0084, Longitude = 28.9779, Category = PoiCategory.Müze }
            );

            // TODO: Gelecekte diğer tablolar için de Seeding veya özel kurallar buraya eklenebilir.
        }
    }
}
