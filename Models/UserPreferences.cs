using System.ComponentModel.DataAnnotations; // 'Key' niteliğini kullanmak için gerekli.
using System.Collections.Generic; // Listeler gibi koleksiyonları kullanmak için gerekli, her ne kadar burada string olarak saklasak da.

/// <summary>
/// Uygulama kullanıcılarının kişiselleştirilmiş gezi rotası tercihlerini veritabanında saklamak için kullanılan veri modelidir.
/// Bu sınıf, veritabanında 'UserPreferences' adında bir tabloya karşılık gelecektir.
/// </summary>
public class UserPreferences
{
    /// <summary>
    /// Bu tercih setinin veritabanındaki benzersiz kimliği (Primary Key).
    /// Her yeni tercih kaydı için otomatik olarak artan bir sayı atanır.
    /// </summary>
    [Key] // Bu özelliğin veritabanında birincil anahtar olduğunu belirtir.
    public int Id { get; set; }

    /// <summary>
    /// Bu tercihlerin hangi kullanıcıya ait olduğunu gösteren kullanıcı kimliği.
    /// Kullanıcı oturum açtığında JWT token'ından alınacak ve buraya kaydedilecektir.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Kullanıcının gezi için ayırdığı bütçe tercihi.
    /// Frontend'den gelen değere göre (örn: 0, 1, 2) bütçe seviyesini temsil edebilir.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Kullanıcının gezi rotası için tercih ettiği kalabalık düzeyi.
    /// Örneğin: 0 = Az kalabalık, 1 = Orta, 2 = Kalabalık.
    /// </summary>
    public int CrowdednessPreference { get; set; }

    /// <summary>
    /// Kullanıcının "Yürüyüş" ulaşım modu seçeneğinde kabul edebileceği maksimum yürüme mesafesi (metre cinsinden).
    /// Frontend'deki "Yürüyüş" seçeneği ile ilişkili olacak.
    /// </summary>
    public int MaxWalkDistance { get; set; }

    /// <summary>
    /// Kullanıcının seçtiği ilgi alanı temalarının virgülle ayrılmış bir listesi (örn: "Sanat,Tarih,Yemek").
    /// Frontend'deki "Kategori Seçimi" kısmından gelir. Veritabanında tek bir string olarak saklanır.
    /// </summary>
    public string PreferredThemes { get; set; } = string.Empty; // Boş olmaması için varsayılan değer atanır.

    /// <summary>
    /// Kullanıcının gezi rotası için tercih ettiği ulaşım modu (örn: "Yürüyüş", "Bisiklet", "Araç", "Toplu Taşıma").
    /// Frontend'deki "Ulaşım Türü" kısmından gelir.
    /// </summary>
    public string PreferredTransportationMode { get; set; } = string.Empty; // Boş olmaması için varsayılan değer atanır.

    /// <summary>
    /// Kullanıcının rotası için istediği minimum başlangıç saati (örneğin: 8 = sabah 08:00).
    /// Frontend'deki "Zaman Aralığı" kısmının başlangıcı.
    /// </summary>
    public int MinStartTimeHour { get; set; }

    /// <summary>
    /// Kullanıcının rotası için istediği maksimum bitiş saati (örneğin: 18 = akşam 18:00).
    /// Frontend'deki "Zaman Aralığı" kısmının bitişi.
    /// </summary>
    public int MaxEndTimeHour { get; set; }

    /// <summary>
    /// Kullanıcının rotasına dahil edilecek ilgi çekici noktaların (POI) sahip olması gereken minimum puan.
    /// Bu, rotanın kalitesini etkileyen bir filtre olabilir.
    /// </summary>
    public double MinPoiRating { get; set; }

    /// <summary>
    /// Kullanıcının trafik durumunun dikkate alınıp alınmadığı tercihi.
    /// Frontend'deki "Trafik durumunu dikkate al" onay kutusuna karşılık gelir.
    /// </summary>
    public bool ConsiderTraffic { get; set; }

    /// <summary>
    /// Kullanıcının en kısa rotanın öncelikli olup olmadığı tercihi.
    /// Frontend'deki "En kısa rotayı öncelikle" onay kutusuna karşılık gelir.
    /// </summary>
    public bool PrioritizeShortestRoute { get; set; }

    /// <summary>
    /// Kullanıcının engelli erişiminin dikkate alınıp alınmadığı tercihi.
    /// Frontend'deki "Engelli erişimi" onay kutusuna karşılık gelir.
    /// </summary>
    public bool AccessibilityFriendly { get; set; }
}