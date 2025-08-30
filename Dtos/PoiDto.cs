namespace GeziRotasi.API.Dtos
{
    public class PoiDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Category { get; set; } = string.Empty;
        public double? AvgRating { get; set; }
        public bool IsOpenNow { get; set; }
        public double DistanceMeters { get; set; }
    }
}
