#nullable enable
using System.Globalization;
using System.Text.Json;
using GeziRotasi.API.Data;
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GeziRotasi.API.Services
{
    public class MapRouteService : IMapRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<MapRouteService> _logger;
        private readonly AppDbContext _db;

        public MapRouteService(HttpClient httpClient, IConfiguration config, ILogger<MapRouteService> logger, AppDbContext db)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _db = db;
        }

        public async Task<RouteResponseDto> GetRouteAsync(RouteRequestDto request, CancellationToken ct = default)
        {
            if (request.Coordinates == null || request.Coordinates.Count < 2)
                throw new ArgumentException("Rota için en az 2 koordinat gerekli.");

            var mode = NormalizeMode(request.Mode);
            var coordsString = string.Join(";", request.Coordinates
                .Select(c => $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

            var url = BuildOsrmUrl(mode, coordsString, request);
            _logger.LogInformation("OSRM URL: {Url}", url);

            var osrmBody = await FetchOsrmAsync(url, ct);
            var route = ParseOsrm(osrmBody);

            route.StartLocation = new LocationDto
            {
                Latitude = request.Coordinates[0][1],
                Longitude = request.Coordinates[0][0],
                Label = "Başlangıç"
            };
            route.EndLocation = new LocationDto
            {
                Latitude = request.Coordinates[^1][1],
                Longitude = request.Coordinates[^1][0],
                Label = "Bitiş"
            };

            return route;
        }

        public async Task<RouteResponseDto> GetRouteWithPoisAsync(RouteRequestDto request, CancellationToken ct = default)
        {
            if (request.Coordinates == null || request.Coordinates.Count == 0)
                throw new ArgumentException("En az 1 koordinat gerekli.");

            _logger.LogInformation("poi ve rota service try → input coords: {@Coords}", request.Coordinates);

            RouteResponseDto route;

            // rota çiz
            if (request.Coordinates.Count >= 2)
            {
                var mode = NormalizeMode(request.Mode);
                var coordsString = string.Join(";", request.Coordinates
                    .Select(c => $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

                var url = BuildOsrmUrl(mode, coordsString, request);
                _logger.LogInformation("OSRM URL: {Url}", url);

                var osrmBody = await FetchOsrmAsync(url, ct);
                route = ParseOsrm(osrmBody);

                route.StartLocation = new LocationDto
                {
                    Latitude = request.Coordinates[0][1],
                    Longitude = request.Coordinates[0][0],
                    Label = "Başlangıç"
                };
                route.EndLocation = new LocationDto
                {
                    Latitude = request.Coordinates[^1][1],
                    Longitude = request.Coordinates[^1][0],
                    Label = "Bitiş"
                };
            }
            else
            {
                // sadece başlangıç
                route = new RouteResponseDto
                {
                    Distance = 0,
                    Duration = 0,
                    Alternatives = new List<RouteVariantDto>(),
                    StartLocation = new LocationDto
                    {
                        Latitude = request.Coordinates[0][1],
                        Longitude = request.Coordinates[0][0],
                        Label = "Başlangıç"
                    }
                };
            }

            // --- POI'leri ekle ---
            try
            {
                var startLon = request.Coordinates[0][0];
                var startLat = request.Coordinates[0][1];
                var searchRadius = request.SearchRadius > 0 ? request.SearchRadius : 3000; // default 3 km

                // EF Core'dan raw liste çek → RAM'de Haversine uygula
                var poisFromDb = await _db.Pois.AsNoTracking().ToListAsync(ct);

                var nearbyPois = poisFromDb
                    .Select(p => new
                    {
                        Entity = p,
                        AvgRating = _db.Reviews
                            .Where(r => r.PoiId == p.Id)
                            .Average(r => (double?)r.Rating) ?? 0,
                        Distance = Haversine(startLat, startLon, p.Latitude, p.Longitude)
                    })
                    .Where(x => x.Distance <= searchRadius)
                    .OrderByDescending(x => x.AvgRating)
                    .ThenBy(x => x.Distance)
                    .ToList();

                route.VisitPois = nearbyPois.Select(x => new PoiDto
                {
                    Id = x.Entity.Id,
                    Name = x.Entity.Name,
                    Description = x.Entity.Description ?? "",
                    Latitude = x.Entity.Latitude,
                    Longitude = x.Entity.Longitude,
                    Category = x.Entity.Category.ToString(),
                    AvgRating = x.AvgRating,
                    IsOpenNow = true,
                    DistanceMeters = x.Distance
                }).ToList();

                _logger.LogInformation("poi ve rota service → {Count} POI bulundu", route.VisitPois.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POI sorgusu sırasında hata.");
                route.VisitPois = new List<PoiDto>();
            }

            return route;
        }

        public async Task<int> SaveRouteAsync(RouteResponseDto dto, int userId, double[] start, double[]? end, CancellationToken ct)
        {
            var route = new Models.Route
            {
                UserId = userId,
                StartLocation = $"{start[0]},{start[1]}",
                EndLocation = end != null ? $"{end[0]},{end[1]}" : "",
                Distance = dto.Distance,
                Duration = dto.Duration,
                Geometry = dto.Geometry?.ToString() ?? "{}",
                CreatedAt = DateTime.UtcNow
            };

            _db.Routes.Add(route);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("rota kaydedildi {RouteId}", route.Id);
            return route.Id;
        }

        // --- Helper'lar ---
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // metre
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180);

        private string BuildOsrmUrl(string mode, string coordsString, RouteRequestDto request)
        {
            var geometries = request.GeoJson ? "geojson" : "polyline";
            string baseUrl = mode switch
            {
                "driving" => (_config["Osrm:CarBaseUrl"] ?? "http://localhost:5002"),
                "foot" => (_config["Osrm:FootBaseUrl"] ?? "http://localhost:5003"),
                _ => (_config["Osrm:BaseUrl"] ?? "http://localhost:8081")
            };

            return $"{baseUrl}/route/v1/{mode}/{coordsString}?overview=full&geometries={geometries}";
        }

        private async Task<string> FetchOsrmAsync(string url, CancellationToken ct)
        {
            using var resp = await _httpClient.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"OSRM {resp.StatusCode}: {resp.ReasonPhrase}");

            return body;
        }

        private RouteResponseDto ParseOsrm(string body)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("routes", out var items) || items.GetArrayLength() == 0)
                throw new InvalidOperationException("OSRM yanıtı boş.");

            var first = items[0];

            return new RouteResponseDto
            {
                Distance = first.GetProperty("distance").GetDouble(),
                Duration = first.GetProperty("duration").GetDouble(),
                Geometry = JsonSerializer.Deserialize<JsonElement>(
                    first.GetProperty("geometry").GetRawText()
                ),
                Alternatives = new List<RouteVariantDto>
                {
                    new RouteVariantDto
                    {
                        Distance = first.GetProperty("distance").GetDouble(),
                        Duration = first.GetProperty("duration").GetDouble(),
                        Geometry = JsonSerializer.Deserialize<JsonElement>(
                            first.GetProperty("geometry").GetRawText()
                        ),
                        IsPrimary = true
                    }
                }
            };
        }

        private static string NormalizeMode(string? mode) =>
            (mode ?? "driving").Trim().ToLowerInvariant() switch
            {
                "car" or "driving" => "driving",
                "foot" or "walk" or "walking" => "foot",
                "bike" or "cycling" => "cycling",
                _ => "driving"
            };
    }
}
