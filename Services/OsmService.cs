using GeziRotasi.API.Dtos;
using System.Globalization;
using System.Net.Http.Json;

namespace GeziRotasi.API.Services
{
    public class OsmService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OsmService> _logger;

        public OsmService(IHttpClientFactory httpClientFactory, ILogger<OsmService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // --- Yakındaki yerleri getir ---
        public async Task<OverpassResponse> GetPlacesAsync(double latitude, double longitude, string placeType, int radius = 1000)
        {
            // Sayıları noktalı formatla çevir (OSM için şart)
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            // Overpass API sorgusu
            var overpassQuery = $@"
                [out:json][timeout:60];
                (
                  node[""amenity""=""{placeType}""](around:{radius},{latStr},{lonStr});
                  way[""amenity""=""{placeType}""](around:{radius},{latStr},{lonStr});
                );
                out body;
                >;
                out skel qt;
            ";

            var client = _httpClientFactory.CreateClient();
            var overpassApiUrl = "https://overpass-api.de/api/interpreter";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", overpassQuery)
            });

            _logger.LogInformation("Overpass Query: {Query}", overpassQuery);

            var response = await client.PostAsync(overpassApiUrl, content);

            // Eğer API başarısız dönerse logla
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                _logger.LogError("Overpass API hatası: {StatusCode} - {Message}", response.StatusCode, errorMsg);
                response.EnsureSuccessStatusCode(); // Exception fırlatır
            }

            var responseString = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Overpass Response: {Response}", responseString);

            var result = await response.Content.ReadFromJsonAsync<OverpassResponse>();
            return result!;
        }

        // --- Tek bir mekanın detayını getir ---
        public async Task<OsmElement?> GetPlaceDetailsAsync(long elementId)
        {
            var client = _httpClientFactory.CreateClient();
            var overpassApiUrl = "https://overpass-api.de/api/interpreter";

            var overpassQuery = $@"
                [out:json];
                (
                  node({elementId});
                  way({elementId});
                );
                out body;
            ";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", overpassQuery)
            });

            var response = await client.PostAsync(overpassApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                _logger.LogError("Overpass API detay sorgusu hatası: {StatusCode} - {Message}", response.StatusCode, errorMsg);
                response.EnsureSuccessStatusCode();
            }

            var result = await response.Content.ReadFromJsonAsync<OverpassResponse>();
            return result?.Elements.FirstOrDefault();
        }
    }
}
