using GeZentiRotasi.Api.Dtos;
using GeZentiRotasi.Api.Services;
using Microsoft.AspNetCore.Mvc;
//conflict test
namespace GeZentiRotasi.Api.Controllers


namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("route")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        public RouteController(IRouteService routeService) => _routeService = routeService;

        /// <summary>OptimizeOrder=true ise /trip, değilse /route çağrılır.</summary>
        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] RouteRequestDto request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var result = await _routeService.GetOptimizedRouteAsync(request,
                                                                    ct);
            var result = await _routeService.GetOptimizedRouteAsync(request, ct);
            return Ok(result);
        }

        // --- ROTA DEĞERLENDİRME METODU ---
        
        [HttpPost("{routeId}/feedback")]
        public IActionResult SubmitRouteFeedback(int routeId, [FromBody] CreateRouteFeedbackDto feedbackDto)
        {
            // Frontend'den gelen verilerin doğru ulaşıp ulaşmadığını kontrol edelim.
            Console.WriteLine($"Gelen Rota Puanı: {feedbackDto.Rating}");
            Console.WriteLine($"Gelen Genel Yorum: {feedbackDto.Comment}");
            Console.WriteLine($"Değerlendirilen Rota ID: {routeId}");
            
            // Frontend'e işlemin başarılı olduğuna dair bir mesaj gönder.
            return Ok(new { message = "Geri bildiriminiz için teşekkür ederiz!" });
        }
    }
}
