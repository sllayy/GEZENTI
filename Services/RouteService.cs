#nullable enable
using System.Globalization;
using System.Text.Json;
using GeziRotasi.API.Data;
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GeziRotasi.API.Services
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<RouteService> _logger;
        private readonly AppDbContext _db;
        private readonly string _osrmBaseUrl;

        public RouteService(HttpClient httpClient, IConfiguration config, ILogger<RouteService> logger, AppDbContext db)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _db = db;
            _osrmBaseUrl = (_config["Osrm:BaseUrl"] ?? "http://localhost:8081").TrimEnd('/');
        }

        public async Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default)
        {
            try
            {
                if (request.Coordinates is null || request.Coordinates.Count < 2)
                    throw new ArgumentException("Rota için en az iki koordinat gereklidir.");

                // --- Kullanıcı tercihleri ---
                var prefs = await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

                if (prefs is not null)
                {
                    if (!string.IsNullOrWhiteSpace(prefs.PreferredTransportationMode))
                        request.Mode = prefs.PreferredTransportationMode;

                    if (prefs.PrioritizeShortestRoute)
                        request.OptimizeOrder = true;

                    if (prefs.MaxWalkDistance > 0 && request.Mode.Equals("foot", StringComparison.OrdinalIgnoreCase))
                        _logger.LogInformation("👟 Max yürüyüş mesafesi: {MaxWalk}", prefs.MaxWalkDistance);

                    var nowHour = DateTime.Now.Hour;
                    if (nowHour < prefs.MinStartTimeHour || nowHour > prefs.MaxEndTimeHour)
                        throw new InvalidOperationException("Seçilen saat kullanıcı tercihlerine uymuyor.");

                    if (prefs.ConsiderTraffic)
                        _logger.LogInformation("🚦 Trafik dikkate alınması istendi (OSRM native desteklemez).");
                }

                // --- POI Query başlangıcı ---
                var poiQuery = _db.Pois.AsQueryable();

                if (prefs != null && !string.IsNullOrEmpty(prefs.PreferredThemes))
                {
                    var selectedThemes = prefs.PreferredThemes.Split(',')
                                                            .Select(t => t.Trim())
                                                            .Where(t => !string.IsNullOrEmpty(t))
                                                            .ToList();

                    _logger.LogInformation("🎯 Kullanıcı seçtiği temalar: {Themes}", string.Join(", ", selectedThemes));

                    if (selectedThemes.Any())
                    {
                        var categoryMap = new Dictionary<string, List<PoiCategory>>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "tarih", new List<PoiCategory> { PoiCategory.Tarih, PoiCategory.Müze, PoiCategory.KültürelTesisler } },
                            { "yemek", new List<PoiCategory> { PoiCategory.Yemek, PoiCategory.Restoran } },
                            { "doğa", new List<PoiCategory> { PoiCategory.Doğa, PoiCategory.Park } },
                            { "eğlence", new List<PoiCategory> { PoiCategory.Eğlence, PoiCategory.Müzik } }
                        };

                        var allowedCategories = new List<PoiCategory>();

                        foreach (var theme in selectedThemes)
                        {
                            if (categoryMap.TryGetValue(theme.ToLower(), out var mapped))
                                allowedCategories.AddRange(mapped);
                            else if (Enum.TryParse<PoiCategory>(theme, true, out var parsed))
                                allowedCategories.Add(parsed);
                        }

                        _logger.LogInformation("🎯 AllowedCategories: {Allowed}", string.Join(", ", allowedCategories));

                        poiQuery = poiQuery.Where(p => allowedCategories.Contains(p.Category));
                    }
                }

                // --- POI + ortalama rating ---
                var poisForRoute = await poiQuery
                    .Select(p => new
                    {
                        Poi = p,
                        AvgRating = _db.Reviews.Where(r => r.PoiId == p.Id)
                                               .Average(r => (double?)r.Rating) ?? 0
                    })
                    .ToListAsync(ct);

                // --- Süreye göre kısıtlama (30 dk / POI) ---
                if (request.TotalAvailableMinutes > 0)
                {
                    var maxPois = request.TotalAvailableMinutes / 30;
                    poisForRoute = poisForRoute.Take(maxPois).ToList();
                    _logger.LogInformation("⏱ Süre kısıtlaması: {MaxPois} POI seçildi", maxPois);
                }

                // --- Hard limit ---
                const int hardLimit = 20;
                if (poisForRoute.Count > hardLimit)
                {
                    poisForRoute = poisForRoute.Take(hardLimit).ToList();
                    _logger.LogWarning("⚠️ Hard limit uygulandı: {HardLimit} POI seçildi.", hardLimit);
                }

                // --- Koordinatlar ---
                var finalCoords = new List<double[]>();
                var start = request.Coordinates.First();
                finalCoords.Add(new[] { start[0], start[1] });

                foreach (var poi in poisForRoute)
                    finalCoords.Add(new[] { poi.Poi.Longitude, poi.Poi.Latitude });

                var end = request.Coordinates.Last();
                finalCoords.Add(new[] { end[0], end[1] });

                var coordsString = string.Join(";", finalCoords.Select(c =>
                    $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

                var url = BuildOsrmUrl(NormalizeMode(request.Mode), coordsString, request);
                _logger.LogInformation("🌍 OSRM URL: {Url}", url);

                var body = await FetchOsrmAsync(url, ct);
                var response = ParseOsrm(body, request.OptimizeOrder);

                // --- VisitPois doldur ---
                response.VisitPois = poisForRoute.Select(x => new PoiDto
                {
                    Id = x.Poi.Id,
                    Name = x.Poi.Name,
                    Description = x.Poi.Description ?? "",
                    Latitude = x.Poi.Latitude,
                    Longitude = x.Poi.Longitude,
                    Category = x.Poi.Category.ToString(),
                    AvgRating = x.AvgRating,
                    IsOpenNow = true,
                    DistanceMeters = 0
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Rota oluşturulurken hata: {@Request}", request);
                throw;
            }
        }

        // --- YENİ EKLENEN METOT ---
        public async Task<IEnumerable<PastRouteDto>> GetRoutesByUserIdAsync(int userId, CancellationToken ct = default)
        {
            try
            {
                var routes = await _db.Routes
                    .AsNoTracking()
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new PastRouteDto
                    {
                        StartLocation = r.StartLocation,
                        EndLocation = r.EndLocation,
                        Distance = r.Distance,
                        Duration = r.Duration,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync(ct);

                return routes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı ID {UserId} için geçmiş rotalar getirilirken hata oluştu.", userId);
                throw;
            }
        }

        private async Task<string> FetchOsrmAsync(string url, CancellationToken ct)
        {
            using var resp = await _httpClient.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ OSRM {Code}. Body: {Body}", (int)resp.StatusCode, Truncate(body, 200));
                throw new InvalidOperationException(
                    $"OSRM {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {Truncate(body, 500)}");
            }
            return body;
        }

        private RouteResponseDto ParseOsrm(string body, bool isTrip)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var key = isTrip ? "trips" : "routes";

            if (!root.TryGetProperty(key, out var items) ||
                items.ValueKind != JsonValueKind.Array ||
                items.GetArrayLength() == 0)
            {
                throw new InvalidOperationException($"OSRM {key} yanıtı beklenen formatta değil veya boş.");
            }

            var item = items[0];
            var distance = item.GetProperty("distance").GetDouble();
            var duration = item.GetProperty("duration").GetDouble();
            var geometry = item.GetProperty("geometry").Clone();

            List<int>? order = null;
            if (isTrip && root.TryGetProperty("waypoints", out var wps) && wps.ValueKind == JsonValueKind.Array)
            {
                order = wps.EnumerateArray()
                           .Select(w => w.TryGetProperty("waypoint_index", out var idx) ? idx.GetInt32() : -1)
                           .Where(i => i >= 0)
                           .ToList();
            }

            return new RouteResponseDto
            {
                Distance = distance,
                Duration = duration,
                Geometry = geometry,
                WaypointOrder = order
            };
        }

        private static string NormalizeMode(string? mode)
        {
            return (mode ?? "driving").Trim().ToLowerInvariant() switch
            {
                "car" or "driving" => "driving",
                "foot" or "walk" or "walking" => "foot",
                _ => "driving"
            };
        }

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s[..max] + "...";

        private string BuildOsrmUrl(string mode, string coordsString, RouteRequestDto request)
        {
            var geometries = request.GeoJson ? "geojson" : "polyline";

            var baseUrl = mode switch
            {
                "driving" => "http://localhost:5002",
                "foot" => "http://localhost:5003",
                _ => _osrmBaseUrl
            };

            if (request.OptimizeOrder)
            {
                var roundtrip = request.ReturnToStart ? "true" : "false";
                var tail = $"overview=full&geometries={geometries}&roundtrip={roundtrip}&source=first";
                if (!request.ReturnToStart) tail += "&destination=last";
                return $"{baseUrl}/trip/v1/{mode}/{coordsString}?{tail}";
            }
            else
            {
                var query = $"overview=full&geometries={geometries}";
                if (request.Alternatives > 0)
                    query += $"&alternatives={Math.Min(request.Alternatives, 3)}";

                return $"{baseUrl}/route/v1/{mode}/{coordsString}?{query}";
            }
        }
    }
}
