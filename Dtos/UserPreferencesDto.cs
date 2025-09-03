namespace GeziRotasi.API.Dtos
{
    /// <summary>
    /// Kullanıcının kişisel gezi tercihlerini temsil eder.
    /// </summary>
    public class UserPreferencesDto
    {
        /// <summary>
        /// Kullanıcının kalabalık ortamlar için tercihi (0 = hiç önemli değil, 5 = çok önemli).
        /// </summary>
        public int CrowdednessPreference { get; set; }

        /// <summary>
        /// Kullanıcının "Yürüyüş" ulaşım modunu seçtiğinde kabul edebileceği maksimum yürüme mesafesi (metre cinsinden).
        /// </summary>
        public int MaxWalkDistance { get; set; }

        /// <summary>
        /// Kullanıcının seçtiği ilgi alanı temalarının listesi (örn: ["Sanat", "Tarih"]).
        /// </summary>
        public List<string> PreferredThemes { get; set; } = new();

        /// <summary>
        /// Kullanıcının gezi rotası için tercih ettiği ulaşım modu (örn: "Yürüyüş", "Bisiklet", "Araç", "Toplu Taşıma").
        /// </summary>
        public string PreferredTransportationMode { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının rotası için istediği minimum başlangıç saati (örn: 8).
        /// </summary>
        public int MinStartTimeHour { get; set; }

        /// <summary>
        /// Kullanıcının rotası için istediği maksimum bitiş saati (örn: 18).
        /// </summary>
        public int MaxEndTimeHour { get; set; }

        /// <summary>
        /// Kullanıcının rotasına dahil edilecek ilgi çekici noktaların (POI) sahip olması gereken minimum puan.
        /// </summary>
        public double MinPoiRating { get; set; }

        /// <summary>
        /// Trafik durumunun dikkate alınıp alınmadığı tercihi.
        /// </summary>
        public bool ConsiderTraffic { get; set; }

        /// <summary>
        /// En kısa rotanın öncelikli olup olmadığı tercihi.
        /// </summary>
        public bool PrioritizeShortestRoute { get; set; }

        /// <summary>
        /// Engelli erişiminin dikkate alınıp alınmadığı tercihi.
        /// </summary>
        public bool AccessibilityFriendly { get; set; }
    }
}
