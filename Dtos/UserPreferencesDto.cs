using System.Collections.Generic; // List<string> kullanmak için gerekli.

public class UserPreferencesDto
{
    /// <summary>
    /// Bu tercihlerin ait olduğu kullanıcının kimliği. Frontend'in hangi kullanıcının tercihlerini görüntülediğini bilmesi için.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Kullanıcının gezi için ayırdığı bütçe tercihi.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Kullanıcının kalabalık ortamlar için tercihi.
    /// </summary>
    public int CrowdednessPreference { get; set; }

    /// <summary>
    /// Kullanıcının "Yürüyüş" ulaşım modunu seçtiğinde kabul edebileceği maksimum yürüme mesafesi (metre cinsinden).
    /// </summary>
    public int MaxWalkDistance { get; set; }

    /// <summary>
    /// Kullanıcının seçtiği ilgi alanı temalarının listesi (örn: ["Sanat", "Tarih"]).
    /// </summary>
    public List<string> PreferredThemes { get; set; } = new List<string>();

    /// <summary>
    /// Kullanıcının gezi rotası için tercih ettiği ulaşım modu (örn: "Yürüyüş", "Bisiklet", "Araç", "Toplu Taşıma").
    /// </summary>
    public string PreferredTransportationMode { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının rotası için istediği minimum başlangıç saati (örneğin: 8).
    /// </summary>
    public int MinStartTimeHour { get; set; }

    /// <summary>
    /// Kullanıcının rotası için istediği maksimum bitiş saati (örneğin: 18).
    /// </summary>
    public int MaxEndTimeHour { get; set; }

    /// <summary>
    /// Kullanıcının rotasına dahil edilecek ilgi çekici noktaların (POI) sahip olması gereken minimum puan.
    /// </summary>
    public double MinPoiRating { get; set; }

    /// <summary>
    /// Kullanıcının trafik durumunun dikkate alınıp alınmadığı tercihi.
    /// </summary>
    public bool ConsiderTraffic { get; set; }

    /// <summary>
    /// Kullanıcının en kısa rotanın öncelikli olup olmadığı tercihi.
    /// </summary>
    public bool PrioritizeShortestRoute { get; set; }

    /// <summary>
    /// Kullanıcının engelli erişiminin dikkate alınıp alınmadığı tercihi.
    /// </summary>
    public bool AccessibilityFriendly { get; set; }
}