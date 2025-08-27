namespace GeziRotasi.API.Models
{
    // Kategorileri temsil eden enum.
    // Arkada planda bunlar birer sayıdır (Yemek=0, Tarih=1, ...).
    public enum PoiCategory
    {
        Yemek,
        Tarih,
        Sahil,
        Kültür,
        Alışveriş,
        Park,
        Müze,
        Restoran,
        Tarihi_Yapı, // Türkçe karakterler yerine Ingilizce karakterler kullanmak daha güvenlidir.
        Genel
    }
}