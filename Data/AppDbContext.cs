using Microsoft.EntityFrameworkCore;
using GeziRotasi.API.Models;
using GeziRotasi.API.Models;
namespace GeziRotasi.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- MEVCUT TABLON ---
        // Bu, projede zaten var olan Mekanlar tablosu.
        public DbSet<Poi> Pois { get; set; }

        // --- YENİ EKLENEN TABLOLAR BURASI ---
        // Senin oluşturduğun Review ve RouteFeedback modellerini veritabanına
        // birer tablo olarak eklemesini söylüyoruz.

        /// <summary>
        /// Mekanlar için yapılan yorum ve puanlamaları tutan tablo.
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// Tamamlanan rotalar için yapılan genel değerlendirmeleri tutan tablo.
        /// </summary>
        public DbSet<RouteFeedback> RouteFeedbacks { get; set; }
        
        // ileride: Users, Preferences vs eklenebilir
    }
}