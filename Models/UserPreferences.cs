using System.ComponentModel.DataAnnotations;

namespace GeziRotasi.API.Models
{
    /// <summary>
    /// Uygulama kullanıcılarının kişiselleştirilmiş gezi rotası tercihlerini veritabanında saklayan entity.
    /// Bu sınıf, veritabanında 'UserPreferences' tablosuna karşılık gelir.
    /// </summary>
    public class UserPreferences
    {
        /// <summary>
        /// Tercih kaydının benzersiz kimliği (Primary Key).
        /// Veritabanında otomatik artan olarak atanır.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tercihlerin hangi kullanıcıya ait olduğunu belirten kullanıcı kimliği.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Kullanıcının kalabalık ortamlar için tercihi.
        /// 0 = hiç önemli değil, 5 = çok önemli.
        /// </summary>
        public int CrowdednessPreference { get; set; }

        /// <summary>
        /// Yürüyüş modunda kabul edilebilecek maksimum mesafe (metre cinsinden).
        /// </summary>
        public int MaxWalkDistance { get; set; }

        /// <summary>
        /// Kullanıcının seçtiği ilgi alanı temalarının virgülle ayrılmış hali.
        /// Örn: "Sanat,Tarih,Yemek".
        /// </summary>
        public string PreferredThemes { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının tercih ettiği ulaşım modu.
        /// Örn: "Yürüyüş", "Bisiklet", "Araç", "Toplu Taşıma".
        /// </summary>
        public string PreferredTransportationMode { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının rotası için istediği minimum başlangıç saati (örn: 8 = sabah 08:00).
        /// </summary>
        public int MinStartTimeHour { get; set; }

        /// <summary>
        /// Kullanıcının rotası için istediği maksimum bitiş saati (örn: 18 = akşam 18:00).
        /// </summary>
        public int MaxEndTimeHour { get; set; }

        /// <summary>
        /// Rota içine dahil edilecek POI’lerin sahip olması gereken minimum puan.
        /// </summary>
        public double MinPoiRating { get; set; }

        /// <summary>
        /// Trafik durumunun dikkate alınıp alınmadığını belirtir.
        /// </summary>
        public bool ConsiderTraffic { get; set; }

        /// <summary>
        /// En kısa rotanın öncelikli olup olmadığını belirtir.
        /// </summary>
        public bool PrioritizeShortestRoute { get; set; }

        /// <summary>
        /// Engelli erişiminin dikkate alınıp alınmadığını belirtir.
        /// </summary>
        public bool AccessibilityFriendly { get; set; }
    }
}
