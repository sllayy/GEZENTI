using GeziRotasi.API.Dtos;
using System.Globalization;
using System.Net.Http.Json;

namespace GeziRotasi.API.Services // Namespace'i kendi projenize göre güncelledim.
{
    public class OsmService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor: Bu servis oluşturulduğunda ona bir HttpClientFactory verilecek.
        public OsmService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OverpassResponse> GetPlacesAsync(double latitude, double longitude, string placeType, int radius = 1000)
        {
            // --- KESİN ÇÖZÜM: KÜLTÜR (NOKTA/VİRGÜL) PROBLEMİNİ GİDERME ---
            // Sayıları, her zaman nokta kullanacak şekilde metne çeviriyoruz.
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            // Sorgu metnini, virgül yerine nokta kullandığından emin olduğumuz bu yeni metinlerle oluşturuyoruz.
            // Ayrıca isteğin takılıp kalmaması için bir timeout ekledik.
            var overpassQuery = $@"
                [out:json][timeout:25];
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

            // Veriyi "data" anahtarıyla, API'nin beklediği formatta hazırlıyoruz.
            var formData = new[]
            {
                new KeyValuePair<string, string>("data", overpassQuery)
            };
            var content = new FormUrlEncodedContent(formData);

            // API'ye isteği gönderiyoruz.
            var response = await client.PostAsync(overpassApiUrl, content);

            // Bu satır artık hata vermeyecek.
            response.EnsureSuccessStatusCode();

            // Gelen JSON'ı C# nesnelerine çeviriyoruz.
            var result = await response.Content.ReadFromJsonAsync<OverpassResponse>();

            return result;
        }
    }
}

