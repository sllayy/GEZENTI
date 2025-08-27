namespace GeZentiRotasi.Api.Dtos
namespace GeziRotasi.API.Dtos
{
    public class RouteRequestDto
    {
        public string Mode { get; set; } = "driving";
        public List<double[]> Coordinates { get; set; } = new();
        public bool OptimizeOrder { get; set; } = true;
        public bool GeoJson { get; set; } = true;
        public bool ReturnToStart { get; set; } = false;
        public bool AvoidHighwaysOnFoot { get; set; } = true;    
        public int Alternatives { get; set; } = 0;
    }
}
