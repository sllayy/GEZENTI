using GeziRotasi.API.Dtos;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/route")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        /// <summary>
        /// Verilen POI'lar ve kullanıcı tercihleri doğrultusunda optimize edilmiş rota oluşturur.
        /// OptimizeOrder=true ise /trip, değilse /route OSRM endpoint'ini çağırır.
        /// </summary>
        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] RouteRequestDto request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _routeService.GetOptimizedRouteAsync(request, ct);
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcının oluşturulan rota için verdiği puan ve yorumu alır.
        /// </summary>
        [HttpPost("{routeId:int}/feedback")]
        public IActionResult SubmitRouteFeedback(int routeId, [FromBody] CreateRouteFeedbackDto feedbackDto)
        {
            _logger.LogInformation("RotaID: {RouteId}, Puan: {Rating}, Yorum: {Comment}",
                routeId, feedbackDto.Rating, feedbackDto.Comment);

            // TODO: Veritabanına kaydetme işlemi için RouteService kullanılabilir.
            return Ok(new { message = "Geri bildiriminiz için teşekkür ederiz!" });
        }
    }
}