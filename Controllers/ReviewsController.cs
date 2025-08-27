using Microsoft.AspNetCore.Mvc; // ASP.NET Core'un temel parçalarını kullanabilmek için.
using GeziRotasi.API.Models;
using GeziRotasi.API.Dtos;      // Az önce oluşturduğumuz DTO'yu tanıyabilmesi için.
using GeziRotasi.API.Models;    // Review modelini tanıyabilmesi için.
namespace GeziRotasi.API.Controllers;

// Bu satırlar, bu sınıfın bir API olduğunu ve adresinin nasıl olacağını belirtir.
[ApiController]
[Route("api/pois/{poiId}/reviews")]
public class ReviewsController : ControllerBase
{
    // --- YENİ YORUM EKLEME BEYNİ ---
    // [HttpPost] -> Bu metodun, veri EKLEMEK için kullanılan bir POST isteğini dinlediğini söyler.
    [HttpPost]
    public IActionResult CreateReviewForPoi(int poiId, [FromBody] CreateReviewDto reviewDto)
    {
        // Parametrelerin Anlamı:
        // int poiId              -> Adresteki {poiId} değişkeninden gelen sayıyı alır. (Örn: 123)
        // [FromBody] CreateReviewDto reviewDto -> Frontend'in yolladığı veri paketini alır.

        // Şimdilik sadece isteğin gelip gelmediğini ve verilerin doğru olduğunu kontrol edelim.
        Console.WriteLine($"Gelen Puan: {reviewDto.Rating}, Gelen Yorum: {reviewDto.Comment}, Mekan ID: {poiId}");
        
        // Frontend'e "İşlem başarılı, yorumun alındı" mesajı gönderiyoruz.
        return Ok(new { message = "Yorum başarıyla oluşturuldu." });
    }

    // --- YORUMLARI LİSTELEME BEYNİ ---
    // [HttpGet] -> Bu metodun, veri GETİRMEK için kullanılan bir GET isteğini dinlediğini söyler.
    [HttpGet]
    public IActionResult GetReviewsForPoi(int poiId)
    {
        // Frontend ekibi test edebilsin diye şimdilik SAHTE (mock) veri gönderelim.
        var fakeReviews = new List<object>
        {
            new { userId = 1, rating = 5, comment = "Manzarası harikaydı, kesinlikle tavsiye ederim!" },
            new { userId = 2, rating = 4, comment = "Giriş biraz pahalı ama değer." }
        };
        
        // Bulduğumuz sahte yorumları frontend'e gönderiyoruz.
        return Ok(fakeReviews);
    }
}