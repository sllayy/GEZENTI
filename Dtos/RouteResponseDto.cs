using System.Text.Json;

namespace GeZentiRotasi.Api.Dtos
{
    public class RouteResponseDto
    {
        public double Distance { get; set; }       // metre
        public double Duration { get; set; }       // saniye
        public JsonElement Geometry { get; set; }  // GeoJSON
        public List<int>? WaypointOrder { get; set; }
    }
}
