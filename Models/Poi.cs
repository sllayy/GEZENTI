using System.ComponentModel.DataAnnotations;

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

        public string? ExternalApiId { get; set; }

        [StringLength(255)]
        public string? OpeningHours { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }
    }
}