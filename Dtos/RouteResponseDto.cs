namespace GeziRotasi.API.Dtos
{    
    public class RouteResponseDto
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public object Geometry { get; set; } = default!;
        public List<int>? WaypointOrder { get; set; }      
        public List<RouteVariantDto>? Alternatives { get; set; }
        public bool PreferencesApplied { get; internal set; }
    }
}
