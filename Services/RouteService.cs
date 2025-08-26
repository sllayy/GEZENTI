using System.Globalization;
using System.Text.Json;
using GeZenti.Api.Dtos;
namespace GeZenti.Api.Services
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<RouteService> _logger;

        public RouteService(HttpClient httpClient, IConfiguration config, ILogger<RouteService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default)
        {
            if (request.Coordinates is null || request.Coordinates.Count < 2)
                throw new ArgumentException("En az iki koordinat gerekli.");

            var mode = NormalizeMode(request.Mode);
            var baseUrl = (_config["Osrm:BaseUrl"] ?? "https://router.project-osrm.org").TrimEnd('/');

            // TR locale fix: sayıları noktalı yaz
            var coords = string.Join(";",
                request.Coordinates.Select(c =>
                    $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

            var geometries = request.GeoJson ? "geojson" : "polyline";

            var url = request.OptimizeOrder
                ? $"{baseUrl}/trip/v1/{mode}/{coords}?roundtrip=true&source=first&destination=last&overview=full&geometries={geometries}"
                : $"{baseUrl}/route/v1/{mode}/{coords}?overview=full&geometries={geometries}";

            _logger.LogInformation("OSRM URL: {Url}", url);

            using var resp = await _httpClient.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"OSRM {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {Truncate(body, 500)}");

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (request.OptimizeOrder)
            {
                if (!root.TryGetProperty("trips", out var trips) || trips.ValueKind != JsonValueKind.Array || trips.GetArrayLength() == 0)
                    throw new InvalidOperationException("OSRM trip yanıtı beklenen formatta değil.");

                var trip = trips[0];
                var distance = trip.GetProperty("distance").GetDouble();
                var duration = trip.GetProperty("duration").GetDouble();
                var geometry = trip.GetProperty("geometry");

                List<int>? order = null;
                if (root.TryGetProperty("waypoints", out var wps) && wps.ValueKind == JsonValueKind.Array)
                {
                    order = new List<int>(wps.GetArrayLength());
                    foreach (var w in wps.EnumerateArray())
                        if (w.TryGetProperty("waypoint_index", out var idx) && idx.ValueKind == JsonValueKind.Number)
                            order.Add(idx.GetInt32());
                }

                return new RouteResponseDto
                {
                    Distance = distance,
                    Duration = duration,
                    Geometry = geometry.Clone(),
                    WaypointOrder = order
                };
            }
            else
            {
                if (!root.TryGetProperty("routes", out var routes) || routes.ValueKind != JsonValueKind.Array || routes.GetArrayLength() == 0)
                    throw new InvalidOperationException("OSRM route yanıtı beklenen formatta değil.");

                var route = routes[0];
                return new RouteResponseDto
                {
                    Distance = route.GetProperty("distance").GetDouble(),
                    Duration = route.GetProperty("duration").GetDouble(),
                    Geometry = route.GetProperty("geometry").Clone(),
                    WaypointOrder = null
                };
            }
        }

        private static string NormalizeMode(string? mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return "driving";
            return mode.Trim().ToLowerInvariant() switch
            {
                "car" or "driving" => "driving",
                "foot" or "walk" or "walking" => "foot",
                "bike" or "bicycle" or "cycling" => "cycling",
                _ => "driving"
            };
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "...";
    }
}
