namespace GeziRotasi.API.Dtos
{
    public class RouteFilterDto
    {
        public List<string> Categories { get; set; }
        public double? MinRating { get; set; }
        public double? MaxDistanceKm { get; set; }
        public string Duration { get; set; } // "short", "medium", "long", "multi"
    }
}
