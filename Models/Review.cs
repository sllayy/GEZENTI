namespace GeziRotasi.API.Models
{
    public class Review
    {
        // Property: Bir şablonun içindeki her bir bilgi parçasına "property" denir.
        
        // Her yorumun benzersiz, otomatik artan bir numarası olacak. (Primary Key)
        public int Id { get; set; } 
        
        // Kullanıcının verdiği 1-5 arası yıldız puanı.
        public int Rating { get; set; } 
        
        // Kullanıcının yazdığı yorum metni. Soru işareti "?" bu alanın boş olabileceğini belirtir.
        public string Comment { get; set; } 
        
        // Yorumun ne zaman yapıldığını otomatik olarak kaydeden alan.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        // --- İLİŞKİLER ---
        // Bu iki property, tabloları birbirine bağlamak için çok önemlidir.

        // Bu yorumun hangi mekana (POI) ait olduğunu belirten numara.
        public int PoiId { get; set; }
        
        // Bu yorumu hangi kullanıcının yaptığını belirten numara.
        public int UserId { get; set; } 
    }
}