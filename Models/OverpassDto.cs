using System.Text.Json.Serialization;

namespace GeziRotasi.API.Models // Namespace'i kendi projenize göre güncelledim.
{
    // Gelen ana JSON cevabını karşılayacak sınıf
    public class OverpassResponse
    {
        [JsonPropertyName("elements")] // JSON'daki "elements" alanını bu özelliğe bağlar.
        public List<OsmElement> Elements { get; set; }
    }

    // "elements" dizisindeki her bir mekanı temsil edecek sınıf
    public class OsmElement
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; }
    }
}