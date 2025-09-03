#nullable enable
using System.Collections.Generic;

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
        public string Preference { get; set; } = "fastest";

        /// <summary>
        /// POI arama yarıçapı (metre cinsinden). Default = 3000m
        /// </summary>
        public int SearchRadius { get; set; } = 3000;

        public int UserId { get; set; }

        /// <summary>
        /// Kullanıcının seçtiği rota başlangıç saati (ör. 8)
        /// </summary>
        public int MinStartTimeHour { get; set; } = 8;

        /// <summary>
        /// Kullanıcının seçtiği rota bitiş saati (ör. 18)
        /// </summary>
        public int MaxEndTimeHour { get; set; } = 18;

        /// <summary>
        /// Kullanıcının toplam ayırabileceği süre (dakika cinsinden). 
        /// Örn: 300 = 5 saat
        /// </summary>
        public int TotalAvailableMinutes { get; set; } = 0;

        public UserPreferencesDto? Preferences { get; set; }

    }
}