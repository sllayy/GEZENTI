// GeziRotasi.API/Controllers/ReviewsController.cs

#nullable enable
using Microsoft.AspNetCore.Mvc;
using GeziRotasi.API.Models;
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Services; 
using System;
using System.Threading.Tasks;

namespace GeziRotasi.API.Controllers;

[ApiController]
[Route("api/pois/{poiId}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly PoiService _poiService;

    public ReviewsController(PoiService poiService)
    {
        _poiService = poiService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReviewForPoi(int poiId, [FromBody] CreateReviewDto reviewDto)
    {
        // Gelen DTO'yu, veritabanı modeliniz olan Review'e dönüştürüyoruz.
        var newReview = new Review
        {
            PoiId = poiId,
            Rating = reviewDto.Rating,
            Comment = reviewDto.Comment,
            // Sizin tablonuzdaki 'CreatedAt' kolonuna göre güncellendi.
            CreatedAt = DateTime.UtcNow, 
            // 'Poil' kolonunu kullanmak yerine, ilişkisel modelinize uygun olarak 'PoiId' kullanıyoruz.
            // Sizin tablonuzdaki UserId kolonuna göre güncellendi.
            UserId = 1, // Gerçek bir kullanıcı sistemi olmadığından sabit değer kullanıldı.
            Emoji = reviewDto.Emoji,
        };

        // Yorumu PoiService aracılığıyla veritabanına kaydediyoruz.
        var createdReview = await _poiService.CreateReviewAsync(newReview);

        return Ok(createdReview);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviewsForPoi(int poiId)
    {
        var reviews = await _poiService.GetReviewsForPoiAsync(poiId);
        return Ok(reviews);
    }
}