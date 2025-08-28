using System.Collections.Generic; // List<string> kullanmak için gerekli.
using System.ComponentModel.DataAnnotations; // Veri doğrulama (örn: [Required], [Range]) için gerekli.

namespace GeziRotasi.DTOs 
{
    public class CreateUserPreferencesDto
    {
        /// <summary>
        /// Kullanıcının gezi için ayırdığı bütçe miktarı. Zorunlu bir alandır.
        /// Örneğin: 0 ile 100.000 arasında bir değer.
        /// </summary>
        [Required(ErrorMessage = "Bütçe miktarı zorunludur.")]
        [Range(0, 100000, ErrorMessage = "Bütçe miktarı 0 ile 100.000 arasında olmalıdır.")] // Bütçe miktarı için yeni aralık.
        public decimal Budget { get; set; }

        /// <summary>
        /// Kullanıcının kalabalık ortamlar için tercihi. Zorunlu bir alandır.
        /// Örneğin: 0 = Az kalabalık, 1 = Orta, 2 = Kalabalık.
        /// </summary>
        [Required(ErrorMessage = "Kalabalık tercihi zorunludur.")]
        [Range(0, 2, ErrorMessage = "Kalabalık tercihi 0 ile 2 arasında olmalıdır.")] // Kalabalık seviyesi için örnek aralık.
        public int CrowdednessPreference { get; set; }

        /// <summary>
        /// Kullanıcının seçtiği ilgi alanı temalarının listesi (örn: ["Sanat", "Tarih"]).
        /// Frontend'deki "Kategori Seçimi" kısmından gelir.
        /// </summary>
        public List<string> PreferredThemes { get; set; } = new List<string>();

        /// <summary>
        /// Kullanıcının gezi rotası için tercih ettiği ulaşım modu (örn: "Yürüyüş", "Bisiklet", "Araç", "Toplu Taşıma"). Zorunlu.
        /// Frontend'deki "Ulaşım Türü" kısmından gelir.
        /// </summary>
        [Required(ErrorMessage = "Ulaşım modu zorunludur.")]
        [StringLength(50, ErrorMessage = "Ulaşım modu en fazla 50 karakter olabilir.")]
        public string PreferredTransportationMode { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının "Yürüyüş" ulaşım modunu seçtiğinde kabul edebileceği maksimum yürüme mesafesi (metre cinsinden).
        /// Frontend'deki "Yürüyüş" seçeneği ile alakalı.
        /// </summary>
        [Required(ErrorMessage = "Maksimum yürüme mesafesi zorunludur.")]
        [Range(0, 20000, ErrorMessage = "Maksimum yürüme mesafesi 0 ile 20000 metre arasında olmalıdır.")] // Makul bir yürüme mesafesi aralığı.
        public int MaxWalkDistance { get; set; }

        /// <summary>
        /// Kullanıcının rotası için istediği minimum başlangıç saati (örneğin: 8). Zorunlu.
        /// Frontend'deki "Zaman Aralığı" kısmının başlangıç değeri.
        /// </summary>
        [Required(ErrorMessage = "Minimum başlangıç saati zorunludur.")]
        [Range(0, 23, ErrorMessage = "Minimum başlangıç saati 0 ile 23 arasında olmalıdır.")] // Saat dilimi aralığı.
        public int MinStartTimeHour { get; set; }

        /// <summary>
        /// Kullanıcının rotası için istediği maksimum bitiş saati (örneğin: 18). Zorunlu.
        /// Frontend'deki "Zaman Aralığı" kısmının bitiş değeri.
        /// </summary>
        [Required(ErrorMessage = "Maksimum bitiş saati zorunludur.")]
        [Range(0, 23, ErrorMessage = "Maksimum bitiş saati 0 ile 23 arasında olmalıdır.")] // Saat dilimi aralığı.
        public int MaxEndTimeHour { get; set; }

        /// <summary>
        /// Kullanıcının rotasına dahil edilecek ilgi çekici noktaların (POI) sahip olması gereken minimum puan. Zorunlu.
        /// </summary>
        [Required(ErrorMessage = "Minimum mekan puanı zorunludur.")]
        [Range(0.0, 5.0, ErrorMessage = "Minimum mekan puanı 0.0 ile 5.0 arasında olmalıdır.")] // Puan aralığı 0.0 ile 5.0.
        public double MinPoiRating { get; set; }

        /// <summary>
        /// Kullanıcının trafik durumunun dikkate alınıp alınmadığı tercihi.
        /// Frontend'deki "Trafik durumunu dikkate al" onay kutusu.
        /// </summary>
        public bool ConsiderTraffic { get; set; }

        /// <summary>
        /// Kullanıcının en kısa rotanın öncelikli olup olmadığı tercihi.
        /// Frontend'deki "En kısa rotayı öncelikle" onay kutusu.
        /// </summary>
        public bool PrioritizeShortestRoute { get; set; }

        /// <summary>
        /// Kullanıcının engelli erişiminin dikkate alınıp alınmadığı tercihi.
        /// Frontend'deki "Engelli erişimi" onay kutusu.
        /// </summary>
        public bool AccessibilityFriendly { get; set; }
    }
}