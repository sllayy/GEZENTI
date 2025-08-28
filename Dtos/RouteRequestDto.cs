using System.Collections.Generic; // List<> kullanabilmek için gerekli.
using System.ComponentModel.DataAnnotations; // Validasyon kuralları için gerekli.

// Sadece bir tane, doğru ve tutarlı bir namespace olmalı.
namespace GeziRotasi.API.Dtos
{
    /// <summary>
    /// Rota optimizasyonu için API'ye gönderilecek olan istek modelidir.
    /// </summary>
    public class RouteRequestDto
    {
        public string Mode { get; set; } = "driving"; // driving | foot
        public List<double[]> Coordinates { get; set; } = new();

        // /trip mi /route mu?
        public bool OptimizeOrder { get; set; } = false;
        public bool ReturnToStart { get; set; } = false;

        // /route için alternatif sayısı (0-3)
        public int Alternatives { get; set; } = 0;

        // GeoJSON geometri istiyorsak
        public bool GeoJson { get; set; } = true;        

        // “NoRoute” vakalarını azaltmak için
        public bool SnapToNetwork { get; set; } = true;

        // alternatives dönerse birincil seçimi etkiler: fastest|shortest|balanced
        public string? Preference { get; set; } = "fastest";

        // nearest için yarıçaplar (metre); opsiyonel
        public int[]? Radiuses { get; set; }
    }
}
