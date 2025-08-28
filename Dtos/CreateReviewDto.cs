namespace GeziRotasi.API.Dtos
{
    public class CreateReviewDto
    {
        // Kullanıcıdan sadece bu iki bilgiyi göndermesini istiyoruz.
        // Geri kalan UserId, PoiId gibi bilgileri biz kendimiz kodun içinde halledeceğiz.
        
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}