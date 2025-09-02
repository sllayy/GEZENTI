namespace GeziRotasi.API.Models
{
    // "RouteFeedback" adında yeni bir şablon oluşturuyoruz.
    public class RouteFeedback
    {
        // Her geri bildirimin benzersiz kimliği.
        public int Id { get; set; }

        // Rota deneyimine verilen 1-5 arası puan.
        public int Rating { get; set; }

        // "Bizi Değerlendirin" kutusuna yazılan yorum.
        public string Comment { get; set; }

        // Değerlendirmenin yapıldığı tarih.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- İLİŞKİLER ---
        
        // Bu değerlendirme hangi ROTA için yapıldı?
        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;

        // Bu değerlendirmeyi hangi kullanıcı yaptı?
        public int UserId { get; set; }
    }
}