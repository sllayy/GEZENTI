using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GeziRotasi.API.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// Sadece bir tane, doğru ve tutarlı bir namespace olmalı.
namespace GeziRotasi.API.Services
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<RouteService> _logger;
        private readonly string _osrmBaseUrl;

        public RouteService(HttpClient httpClient, IConfiguration config, ILogger<RouteService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            // BaseUrl'i constructor'da bir kez alıp saklamak daha verimlidir.
            _osrmBaseUrl = (_config["Osrm:BaseUrl"] ?? "https://router.project-osrm.org").TrimEnd('/');
        }

        public async Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default)
        {
            if (request.Coordinates is null || request.Coordinates.Count < 2)
            {
                throw new ArgumentException("Rota optimizasyonu için en az iki koordinat gereklidir.");
            }

            var mode = NormalizeMode(request.Mode);
            var url = BuildOsrmUrl(request, mode);
            
            _logger.LogInformation("OSRM URL: {Url}", url);

            var body = await FetchOsrmDataAsync(url, ct);

            return ParseOsrmResponse(body, request.OptimizeOrder);
        }

        // --- YARDIMCI METOTLAR ---

        private string BuildOsrmUrl(RouteRequestDto request, string mode)
        {
            var coords = string.Join(";", 
                request.Coordinates.Select(c => 
                    $"{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}"));

            var geometries = request.GeoJson ? "geojson" : "polyline";
            
            if (request.OptimizeOrder)
            {
                var roundtrip = request.ReturnToStart ? "true" : "false";
                var tail = $"overview=full&geometries={geometries}&roundtrip={roundtrip}&source=first";
                if (!request.ReturnToStart) tail += "&destination=last";
                return $"{_osrmBaseUrl}/trip/v1/{mode}/{coords}?{tail}";
            }
            else
            {
                var query = $"overview=full&geometries={geometries}";
                if (request.Alternatives > 0) query += $"&alternatives={Math.Min(request.Alternatives, 3)}";
                if (mode == "foot" && request.AvoidHighwaysOnFoot) query += "&exclude=motorway,trunk,primary,secondary,tertiary";
                return $"{_osrmBaseUrl}/route/v1/{mode}/{coords}?{query}";
            }
        }
        
        private async Task<string> FetchOsrmDataAsync(string url, CancellationToken ct)
        {
            using var response = await _httpClient.GetAsync(url, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OSRM {Code} ile yanıt verdi. Body: {Body}", (int)response.StatusCode, Truncate(body, 200));
                // OSRM için özel hata yönetimi eklenebilir. Şimdilik genel bir hata fırlatıyoruz.
                throw new InvalidOperationException($"OSRM {(int)response.StatusCode} {response.ReasonPhrase}. Body: {Truncate(body, 500)}");
            }

            return body;
        }

        private RouteResponseDto ParseOsrmResponse(string body, bool isTrip)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var propertyName = isTrip ? "trips" : "routes";
            
            if (!root.TryGetProperty(propertyName, out var items) || items.ValueKind != JsonValueKind.Array || items.GetArrayLength() == 0)
            {
                throw new InvalidOperationException($"OSRM {propertyName} yanıtı beklenen formatta değil veya boş.");
            }

            var item = items[0];
            var distance = item.GetProperty("distance").GetDouble();
            var duration = item.GetProperty("duration").GetDouble();
            var geometry = item.GetProperty("geometry").Clone();
            
            List<int>? waypointOrder = null;
            if (isTrip && root.TryGetProperty("waypoints", out var wps) && wps.ValueKind == JsonValueKind.Array)
            {
                waypointOrder = wps.EnumerateArray()
                                   .Select(w => w.TryGetProperty("waypoint_index", out var idx) ? idx.GetInt32() : -1)
                                   .Where(i => i != -1)
                                   .ToList();
            }

            return new RouteResponseDto
            {
                Distance = distance,
                Duration = duration,
                Geometry = geometry,
                WaypointOrder = waypointOrder
            };
        }

        private static string NormalizeMode(string mode)
        {
            return (mode?.Trim().ToLowerInvariant()) switch
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
