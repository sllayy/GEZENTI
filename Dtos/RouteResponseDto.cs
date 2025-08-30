#nullable enable
namespace GeziRotasi.API.Dtos
{
    /// <summary>
    /// OSRM'den donen rota bilgisini kullan�c�ya sunmak icin kullanilan DTO.
    /// </summary>
    public class RouteResponseDto
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public object Geometry { get; set; } = default!;
        public List<int>? WaypointOrder { get; set; }
        public List<RouteVariantDto>? Alternatives { get; set; }
        public bool PreferencesApplied { get; internal set; }

        public LocationDto StartLocation { get; set; } = default!;
        public LocationDto? EndLocation { get; set; }
        public List<PoiDto> VisitPois { get; set; } = new();
    }
}
