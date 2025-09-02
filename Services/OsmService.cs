using GeziRotasi.API.Dtos; // Namespace'i kendi projenle eşleşmiyorsa güncelle.
using System.Globalization;
using System.Net.Http.Json;

namespace GeziRotasi.API.Services // Namespace'i kendi projenle eşleşmiyorsa güncelle.
{
    public class OsmService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor: .NET'in HttpClient'ı yönetmesi için IHttpClientFactory'i alır.
        public OsmService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // --- BU METOT ZATEN VARDI, DEĞİŞİKLİK YOK ---
        // Yakındaki yerleri aramak için kullanılır.
        public async Task<OverpassResponse> GetPlacesAsync(double latitude, double longitude, string placeType, int radius = 1000)
        {
            // Sayıları API'nin anlayacağı formata (noktalı) çevirir.
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            // Overpass API'ye gönderilecek sorgu metni.
            var overpassQuery = $@"
                [out:json][timeout:25];
                (
                node[""amenity""=""{placeType}""](around:{radius},{latitude},{longitude});
                way[""amenity""=""{placeType}""](around:{radius},{latitude},{longitude});
                );
                out body;
                >;
                out skel qt;
            ";


            var client = _httpClientFactory.CreateClient();
            var overpassApiUrl = "https://overpass-api.de/api/interpreter";

            // Sorguyu API'nin beklediği formatta hazırla.
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", overpassQuery)
            });

            var response = await client.PostAsync(overpassApiUrl, content);
            response.EnsureSuccessStatusCode(); // Hata varsa burada program durur.

            // Gelen JSON cevabını C# nesnelerine çevir.
            var result = await response.Content.ReadFromJsonAsync<OverpassResponse>();
            return result;
        }

        // --- YENİ EKLENEN METOT BURASI ---
        // Tek bir mekanın ID'sini kullanarak detaylarını getirmek için kullanılır.
        public async Task<OsmElement> GetPlaceDetailsAsync(long elementId)
        {
            var client = _httpClientFactory.CreateClient();
            var overpassApiUrl = "https://overpass-api.de/api/interpreter";

            // Overpass Sorgusu: Sadece belirtilen ID'ye sahip olan elemanı (node veya way) getirir.
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
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OverpassResponse>();

            // API'den gelen cevapta bir eleman varsa onu döndürür, yoksa null döner.
            return result?.Elements.FirstOrDefault();
        }
    }
}

// --- BİTİŞ --- KOPYALAMAYI BURADA BİTİR