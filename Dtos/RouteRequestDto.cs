namespace GeZenti.Api.Dtos
{
    public class RouteRequestDto
    {
        public string Mode { get; set; } = "driving";
        public List<double[]> Coordinates { get; set; } = new();
        public bool OptimizeOrder { get; set; } = true;
        public bool GeoJson { get; set; } = true;
    }
}
