#nullable enable
using System.Globalization;
using System.Text.Json;
using GeziRotasi.API.Data;
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GeziRotasi.API.Services
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<RouteService> _logger;
        private readonly AppDbContext _db;

        public RouteService(HttpClient httpClient, IConfiguration config, ILogger<RouteService> logger, AppDbContext db)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _db = db;
        }

        public async Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default)
        {
            // --- doğrulama
            if (request.Coordinates is null || request.Coordinates.Count < 2)
                throw new ArgumentException("Rota için en az iki koordinat gereklidir.");

            // --- tercihler: sadece alan doldurma/ince ayar için (POI seçimi YOK!)
            var prefs = await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
            if (prefs is not null)
            {
                if (!string.IsNullOrWhiteSpace(prefs.PreferredTransportationMode))
                    request.Mode = prefs.PreferredTransportationMode;

                if (prefs.PrioritizeShortestRoute)
                    request.OptimizeOrder = true;

                if (prefs.MaxWalkDistance > 0 && string.Equals(request.Mode, "foot", StringComparison.OrdinalIgnoreCase))
                    _logger.LogInformation("Max yürüyüş mesafesi: {MaxWalk}", prefs.MaxWalkDistance);

                var nowHour = DateTime.Now.Hour;
                if (nowHour < prefs.MinStartTimeHour || nowHour > prefs.MaxEndTimeHour)
                    _logger.LogWarning("Kullanıcı saat tercihi dışında bir çağrı yapıldı: {NowHour}", nowHour);

                if (prefs.ConsiderTraffic)
                    _logger.LogInformation("Trafik dikkate alınması istendi (OSRM native desteklemez).");
            }

            // --- sadece gelen koordinatlar → OSRM
            var mode = NormalizeMode(request.Mode);
            var coordsString = string.Join(";",
                request.Coordinates.Select(c =>
                    $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

            var url = BuildOsrmUrl(mode, coordsString, request);
            _logger.LogInformation("OSRM URL: {Url}", url);

            var body = await FetchOsrmAsync(url, ct);
            var response = ParseOsrm(body, request.OptimizeOrder, request.Preference);
            response.PreferencesApplied = prefs is not null;
            return response;
        }

        private string BuildOsrmUrl(string mode, string coordsString, RouteRequestDto request)
        {
            var geometries = request.GeoJson ? "geojson" : "polyline";

            // moda göre base url (takım arkadaşının değişikliğini koruyoruz)
            string baseUrl = mode switch
            {
                "driving" => (_config["Osrm:CarBaseUrl"] ?? _config["Osrm:BaseUrl"] ?? "http://localhost:5002"),
                "foot" => (_config["Osrm:FootBaseUrl"] ?? _config["Osrm:BaseUrl"] ?? "http://localhost:5003"),
                _ => (_config["Osrm:BaseUrl"] ?? "http://localhost:8081")
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
                if (request.Alternatives > 0) query += $"&alternatives={Math.Min(request.Alternatives, 3)}";
                return $"{baseUrl}/route/v1/{mode}/{coordsString}?{query}";
            }
        }

        private async Task<string> FetchOsrmAsync(string url, CancellationToken ct)
        {
            using var resp = await _httpClient.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("OSRM {Code}. Body: {Body}", (int)resp.StatusCode, Truncate(body, 200));
                throw new InvalidOperationException(
                    $"OSRM {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {Truncate(body, 500)}");
            }
            return body;
        }

        private RouteResponseDto ParseOsrm(string body, bool isTrip, string? preference)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var key = isTrip ? "trips" : "routes";

            if (!root.TryGetProperty(key, out var items) || items.ValueKind != JsonValueKind.Array || items.GetArrayLength() == 0)
                throw new InvalidOperationException($"OSRM {key} yanıtı beklenen formatta değil veya boş.");

            // alternatifleri topla (hem /route hem /trip ortak alanlar: distance, duration, geometry)
            var variants = new List<RouteVariantDto>();
            foreach (var it in items.EnumerateArray())
            {
                variants.Add(new RouteVariantDto
                {
                    Distance = it.GetProperty("distance").GetDouble(),
                    Duration = it.GetProperty("duration").GetDouble(),
                    Geometry = it.GetProperty("geometry").Clone(),
                    IsPrimary = false
                });
            }

            // tercih ile birincil seçimi yap
            int pick = 0;
            if (!string.IsNullOrWhiteSpace(preference))
            {
                var pref = preference.Trim().ToLowerInvariant();
                if (pref is "shortest" or "balanced" or "fastest")
                {
                    double scoreMin = double.MaxValue;
                    for (int i = 0; i < variants.Count; i++)
                    {
                        var v = variants[i];
                        double score = pref switch
                        {
                            "shortest" => v.Distance,
                            "balanced" => v.Distance * 0.5 + v.Duration * 0.5,
                            _ => v.Duration // fastest
                        };
                        if (score < scoreMin) { scoreMin = score; pick = i; }
                    }
                }
            }
            variants[pick].IsPrimary = true;

            // waypoint order (sadece /trip)
            List<int>? order = null;
            if (isTrip && root.TryGetProperty("waypoints", out var wps) && wps.ValueKind == JsonValueKind.Array)
            {
                order = wps.EnumerateArray()
                           .Select(w => w.TryGetProperty("waypoint_index", out var idx) ? idx.GetInt32() : -1)
                           .Where(i => i >= 0)
                           .ToList();
            }

            var primary = variants[pick];
            return new RouteResponseDto
            {
                Distance = primary.Distance,
                Duration = primary.Duration,
                Geometry = primary.Geometry,
                WaypointOrder = order,
                Alternatives = variants
            };
        }

        private static string NormalizeMode(string? mode)
        {
            return (mode ?? "driving").Trim().ToLowerInvariant() switch
            {
                "car" or "driving" => "driving",
                "foot" or "walk" or "walking" => "foot",
                "bike" or "bicycle" or "cycling" => "cycling", // isterseniz kaldırabilirsiniz
                _ => "driving"
            };
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "...";
    }
}
