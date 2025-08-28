using System.Globalization;
using System.Text;
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
        private readonly string _osrmBaseUrl;

        // FE -> DB enum köprüsü
        private readonly Dictionary<string, PoiCategory> _themeMap = new()
        {
            { "Tarih", PoiCategory.Tarih },
            { "Müze", PoiCategory.Müze },
            { "Sanat", PoiCategory.Sanat },
            { "Yemek", PoiCategory.Yemek },
            { "Alışveriş", PoiCategory.Alışveriş },
            { "Doğa", PoiCategory.Doğa },
            { "Eğlence", PoiCategory.Eğlence },
            { "Müzik", PoiCategory.Müzik },
            { "Sahil", PoiCategory.Sahil },
            { "Park", PoiCategory.Park },
        };

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
            if (request.Coordinates is null || request.Coordinates.Count < 2)
                throw new ArgumentException("Rota için en az iki koordinat gereklidir.");

            // 1) Kullanıcı tercihleri
            var prefs = await _db.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

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
                    throw new InvalidOperationException("Seçilen saat kullanıcı tercihlerine uymuyor.");

                if (prefs.ConsiderTraffic)
                    _logger.LogInformation("Trafik dikkate alınması istendi (OSRM native desteklemez).");
            }

            // 2) Tema -> POI filtreleme (opsiyonel)
            List<Poi> poisForRoute;
            if (prefs != null && !string.IsNullOrEmpty(prefs.PreferredThemes))
            {
                var selectedThemes = prefs.PreferredThemes.Split(',')
                                                          .Select(t => t.Trim())
                                                          .Where(t => !string.IsNullOrEmpty(t))
                                                          .ToList();

                var mappedCategories = selectedThemes
                    .Where(t => _themeMap.ContainsKey(t))
                    .Select(t => _themeMap[t])
                    .Distinct()
                    .ToList();

                poisForRoute = mappedCategories.Any()
                    ? await _db.Pois.Where(p => mappedCategories.Contains(p.Category)).ToListAsync(ct)
                    : await _db.Pois.ToListAsync(ct);
            }
            else
            {
                // Tema seçimi yoksa tüm POI’ler (veya boş bırakarak sadece start/end ile rota) — burada tüm POI’leri almak isteğe bağlı.
                poisForRoute = await _db.Pois.ToListAsync(ct);
            }

            // 3) OSRM’e gönderilecek waypoint listesi (start -> POIs -> end)
            //   OptimizeOrder=true ise TRIP endpoint sıralamayı kendi yapar.
            var finalCoords = new List<double[]>();

            // Başlangıç
            var start = request.Coordinates.First();
            finalCoords.Add(new[] { start[0], start[1] });

            // POI’ler (varsa)
            foreach (var poi in poisForRoute)
            {
                // Aynı noktayı iki kez eklemeyelim
                if (!(poi.Longitude == start[0] && poi.Latitude == start[1]))
                    finalCoords.Add(new[] { poi.Longitude, poi.Latitude });
            }

            // Bitiş
            var end = request.Coordinates.Last();
            if (!(end[0] == start[0] && end[1] == start[1]))
                finalCoords.Add(new[] { end[0], end[1] });

            // Eğer filtre sonunda POI yoksa, OSRM en az iki nokta görsün diye start/end kalsın
            if (finalCoords.Count < 2)
            {
                finalCoords.Clear();
                finalCoords.Add(new[] { start[0], start[1] });
                finalCoords.Add(new[] { end[0], end[1] });
            }

            var mode = NormalizeMode(request.Mode);
            var coordsString = string.Join(";",
                finalCoords.Select(c => $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

            var url = BuildOsrmUrl(mode, coordsString, request);
            _logger.LogInformation("OSRM URL: {Url}", url);

            var body = await FetchOsrmAsync(url, ct);
            var response = ParseOsrm(body, request.OptimizeOrder);
            response.PreferencesApplied = prefs is not null;
            return response;
        }         

        private string BuildOsrmUrl(string mode, string coordsString, RouteRequestDto request)
        {
            var geometries = request.GeoJson ? "geojson" : "polyline";

            if (request.OptimizeOrder)
            {
                var roundtrip = request.ReturnToStart ? "true" : "false";
                var tail = $"overview=full&geometries={geometries}&roundtrip={roundtrip}&source=first";
                if (!request.ReturnToStart) tail += "&destination=last";
                return $"{_osrmBaseUrl}/trip/v1/{mode}/{coordsString}?{tail}";
            }
            else
            {
                var query = $"overview=full&geometries={geometries}";
                if (request.Alternatives > 0) query += $"&alternatives={Math.Min(request.Alternatives, 3)}";                

                return $"{_osrmBaseUrl}/route/v1/{mode}/{coordsString}?{query}";
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

        private RouteResponseDto ParseOsrm(string body, bool isTrip)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var key = isTrip ? "trips" : "routes";

            if (!root.TryGetProperty(key, out var items) || items.ValueKind != JsonValueKind.Array || items.GetArrayLength() == 0)
                throw new InvalidOperationException($"OSRM {key} yanıtı beklenen formatta değil veya boş.");

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

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "...";
    }
}
