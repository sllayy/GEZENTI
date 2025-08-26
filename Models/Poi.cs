using System.ComponentModel.DataAnnotations; // Bu satırı eklemek çok önemli!

namespace GeziRotasi.API.Models
{
    public class Poi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim alanı zorunludur.")] // Bu alan boş olamaz
        [StringLength(100, ErrorMessage = "İsim alanı en fazla 100 karakter olabilir.")] // En fazla 100 karakter
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500)] // En fazla 500 karakter
        public string Description { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        public string? ExternalApiId { get; set; }
    }
}
