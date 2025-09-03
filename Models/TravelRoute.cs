namespace GeziRotasi.API.Models
{
    public class TravelRoute
    {
        public int Id { get; set; }

        // Kategorisi (örn: Tarih, Müze, Yemek vs.)
        public string Category { get; set; }

        // Ortalama puan
        public double AverageRating { get; set; }

        // Mesafe km cinsinden
        public double DistanceKm { get; set; }

        // Süre (örn: "short", "medium", "long", "multi")
        public string Duration { get; set; }
    }
}