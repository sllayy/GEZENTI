using System.Text.Json.Serialization;

namespace GeziRotasi.API.Dtos
{
    public class GoogleFirebaseDto
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;
    }
}
