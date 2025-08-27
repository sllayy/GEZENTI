using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace GeziRotasi.API.Models
{
    public class Poi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "İsim alanı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama alanı en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        [Range(-90.0, 90.0, ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır.")]
        public double Latitude { get; set; }

        [Range(-180.0, 180.0, ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır.")]
        public double Longitude { get; set; }

        [StringLength(50)]
        public string Category { get; set; }
        public PoiCategory Category { get; set; }

        public string? ExternalApiId { get; set; }

        [StringLength(255)]
        public string? OpeningHours { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }
        /// Esnek olması için string olarak tutuyoruz.
        [StringLength(100)]
        public string? GirisUcreti { get; set; }

        /// Bu mekanda harcanacak ortalama süre (dakika cinsinden).
        /// Rota optimizasyonu için kullanılabilir.
        public int? OrtalamaZiyaretSuresi { get; set; }

        /// Mekana ait bir görselin URL'si.
        [StringLength(500)]
        public string? ImageUrl { get; set; }


        // Bir POI, birden çok (7 tane) Çalışma Saati kaydına sahip olabilir.
        // Bu, "one-to-many" (bire çok) bir ilişkidir.
        public ICollection<WorkingHour> WorkingHours { get; set; }
        
        // Bir POI, birden çok Özel Gün kaydına sahip olabilir.
        public ICollection<SpecialDayHour> SpecialDayHours { get; set; }
    }
}