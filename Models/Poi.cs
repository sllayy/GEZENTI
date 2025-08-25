namespace GeziRotasi.API.Models
{
    public class Poi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Category { get; set; }

        // Sena'nın kullanması için Google Places API'den gelen ID'yi tutacak alan
        // Soru işareti (?) bu alanın boş olabileceği anlamına gelir.
        public string? ExternalApiId { get; set; }
    }
}