using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeziRotasi.API.Models
{
    public class Poi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "İsim alanı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(-90.0, 90.0, ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır.")]
        public double Latitude { get; set; }

        [Range(-180.0, 180.0, ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır.")]
        public double Longitude { get; set; }

        // Category artık TEK BİR TANE ve doğru tipte: PoiCategory enum
        public PoiCategory Category { get; set; }

        public string ExternalApiId { get; set; }

        [StringLength(255)]
        public string OpeningHours { get; set; }

        [StringLength(255)]
        public string Website { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string GirisUcreti { get; set; }

        public int? OrtalamaZiyaretSuresi { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        // --- İLİŞKİLER ---
        // ICollection<> kullanmak için "using System.Collections.Generic;" gereklidir.
        public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
        public ICollection<SpecialDayHour> SpecialDayHours { get; set; } = new List<SpecialDayHour>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
