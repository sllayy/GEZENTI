namespace GeziRotasi.API.Dtos
{
    public class CreateRouteDto
    {
        public string Category { get; set; }
        public double AverageRating { get; set; }
        public double DistanceKm { get; set; }
        public string Duration { get; set; }
    }
}
