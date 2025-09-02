using GeziRotasi.API.Dtos;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GeziRotasi.API.Controllers
{
    [Authorize] // Artık tüm endpoint'ler kimlik doğrulaması gerektiriyor
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
        /// Verilen POI'lar için optimize edilmiş bir rota oluşturur.
        /// OptimizeOrder=true ise /trip, değilse /route OSRM endpoint'ini çağırır.
        /// </summary>
        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] RouteRequestDto request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // --- Güvenlik kontrolü: token'daki userId ile request içindeki userId eşleşmeli ---
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (request.UserId.ToString() != currentUserId)
            {
                return Forbid("Bu kullanıcı adına işlem yapma yetkiniz yok.");
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

            // TODO: Veritabanına kaydetme işlemi için servis çağırılacak.
            return Ok(new { message = "Geri bildiriminiz için teşekkür ederiz!" });
        }

        // --- YENİ EKLENEN ENDPOINT ---
        /// <summary>
        /// Giriş yapmış kullanıcının geçmiş rotalarını listeler.
        /// </summary>
        [HttpGet("my-routes")]
        public async Task<IActionResult> GetMyRoutes(CancellationToken ct)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            _logger.LogInformation("Kullanıcı {UserId} için geçmiş rotalar isteniyor.", userId);

            var routes = await _routeService.GetRoutesByUserIdAsync(userId, ct);

            return Ok(routes);
        }
    }
}
