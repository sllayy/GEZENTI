using GeziRotasi.API.Dtos;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _service;

    public RecommendationController(RecommendationService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<RecommendationResultDto>> Get(
        int userId, double lat, double lon, double radiusKm = 30, CancellationToken ct = default)
    {
        var result = await _service.GetRecommendedPoiIdsByUserIdAsync(userId, lat, lon, radiusKm, ct);
        return Ok(result);
    }
}
