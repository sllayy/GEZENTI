using System.Globalization;
using System.Text;
using System.Text.Json;
using GeziRotasi.API.Dtos;

namespace GeziRotasi.API.Services
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
            var baseUrl = (_config["Osrm:BaseUrl"] ?? "http://localhost:8081").TrimEnd('/');
                        
            List<double[]> coordsUse = request.Coordinates;
            string?[] hints = Array.Empty<string?>();
            if (request.SnapToNetwork)
            {
                var radiuses = BuildRadiuses(request);
                (coordsUse, hints) = await SnapIfNeededAsync(baseUrl, mode, request.Coordinates, radiuses, ct);
            }

            var coords = string.Join(";", coordsUse.Select(c =>
                $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

            var geometries = request.GeoJson ? "geojson" : "polyline";
                     

            var url = BuildOsrmUrl(baseUrl, mode, coords, geometries, request, hints);

            _logger.LogInformation("OSRM URL: {Url}", url);

            using var resp = await _httpClient.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);            

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"OSRM {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {Truncate(body, 500)}");

            return ParseOsrm(body, request);
        }

        private string BuildOsrmUrl(
            string baseUrl, string mode, string coords, string geometries,
            RouteRequestDto request, string?[] hints)
        {
            
            string hintsParam = "";
            if (hints.Length == request.Coordinates.Count && hints.All(h => !string.IsNullOrEmpty(h)))
            {
                var enc = string.Join(";", hints.Select(Uri.EscapeDataString));
                hintsParam = $"&hints={enc}";
            }

            if (request.OptimizeOrder)
            {
                var roundtrip = request.ReturnToStart ? "true" : "false";
                var tail = new StringBuilder($"overview=full&geometries={geometries}&roundtrip={roundtrip}&source=first");
                if (!request.ReturnToStart) tail.Append("&destination=last");
                if (!string.IsNullOrEmpty(hintsParam)) tail.Append(hintsParam);

                return $"{baseUrl}/trip/v1/{mode}/{coords}?{tail}";
            }
            else
            {
                var sb = new StringBuilder($"overview=full&geometries={geometries}");
                if (request.Alternatives > 0) sb.Append($"&alternatives={Math.Min(request.Alternatives, 3)}");                
                if (!string.IsNullOrEmpty(hintsParam)) sb.Append(hintsParam);

                return $"{baseUrl}/route/v1/{mode}/{coords}?{sb}";
            }
        }

        private async Task<(List<double[]>, string?[])> SnapIfNeededAsync(
            string baseUrl, string mode, List<double[]> coords, int[] radiuses, CancellationToken ct)
        {
            var snapped = new List<double[]>(coords.Count);
            var hints = new string?[coords.Count];

            for (int i = 0; i < coords.Count; i++)
            {
                var lon = coords[i][0].ToString(CultureInfo.InvariantCulture);
                var lat = coords[i][1].ToString(CultureInfo.InvariantCulture);
                var url = $"{baseUrl}/nearest/v1/{mode}/{lon},{lat}?number=1";
                if (radiuses[i] > 0) url += $"&radiuses={radiuses[i]}";

                using var resp = await _httpClient.GetAsync(url, ct);
                resp.EnsureSuccessStatusCode();
                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
                var wp = doc.RootElement.GetProperty("waypoints")[0];

                var loc = wp.GetProperty("location");
                snapped.Add(new[] { loc[0].GetDouble(), loc[1].GetDouble() });

                if (wp.TryGetProperty("hint", out var h) && h.ValueKind == JsonValueKind.String)
                    hints[i] = h.GetString();
            }

            return (snapped, hints);
        }

        private int[] BuildRadiuses(RouteRequestDto req)
        {
            var n = req.Coordinates.Count;
            var def = 200;
            if (req.Radiuses == null || req.Radiuses.Length == 0)
                return Enumerable.Repeat(def, n).ToArray();

            var arr = new int[n];
            for (int i = 0; i < n; i++)
                arr[i] = (i < req.Radiuses.Length && req.Radiuses[i] > 0) ? req.Radiuses[i] : def;
            return arr;
        }

        private RouteResponseDto ParseOsrm(string body, RouteRequestDto request)
        {
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
                    WaypointOrder = order,
                    Alternatives = null
                };
            }
            else
            {
                if (!root.TryGetProperty("routes", out var routes) || routes.ValueKind != JsonValueKind.Array || routes.GetArrayLength() == 0)
                    throw new InvalidOperationException("OSRM route yanıtı beklenen formatta değil.");
                               
                var pref = (request.Preference ?? "fastest").Trim().ToLowerInvariant();
                var (primaryIdx, all) = PickPrimaryByPreference(routes, pref, request.GeoJson);
                var primary = routes[primaryIdx];

                return new RouteResponseDto
                {
                    Distance = primary.GetProperty("distance").GetDouble(),
                    Duration = primary.GetProperty("duration").GetDouble(),
                    Geometry = primary.GetProperty("geometry").Clone(),
                    WaypointOrder = null,
                    Alternatives = all
                };
            }
        }

        private static (int primaryIdx, List<RouteVariantDto> all) PickPrimaryByPreference(
            JsonElement routes, string pref, bool geoJson)
        {
            int n = routes.GetArrayLength();
            var list = new List<RouteVariantDto>(n);
            int best = 0;
            double bestScore = double.MaxValue;

            for (int i = 0; i < n; i++)
            {
                var r = routes[i];
                var dist = r.GetProperty("distance").GetDouble();
                var dur = r.GetProperty("duration").GetDouble();
                object geom = r.GetProperty("geometry").Clone();

                list.Add(new RouteVariantDto
                {
                    Distance = dist,
                    Duration = dur,
                    Geometry = geom,
                    IsPrimary = false
                });

                double score = pref switch
                {
                    "shortest" => dist,
                    "balanced" => dist * 0.5 + dur * 0.5,
                    _ => dur 
                };
                if (score < bestScore) { bestScore = score; best = i; }
            }
            list[best].IsPrimary = true;
            return (best, list);
        }

        private static string NormalizeMode(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return "driving";
            return mode.Trim().ToLowerInvariant() switch
            {
                "car" or "driving" => "driving",
                "foot" or "walk" or "walking" => "foot",
                _ => "driving"
            };
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "...";


    }
}
