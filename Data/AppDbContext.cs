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
    }
}
