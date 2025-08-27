using GeziRotasi.API.Dtos; // DTO'larınızın doğru namespace'i bu olmalı
using GeziRotasi.API.Services; // Servislerinizin doğru namespace'i bu olmalı
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

// Sadece bir tane, doğru namespace olmalı.
namespace GeziRotasi.API.Controllers
{
    [ApiController]
    // Controller'ın adıyla eşleşmesi için genellikle [Route("api/[controller]")] kullanılır.
    // Ama sizin standardınız "route" ise bu da doğrudur.
    [Route("api/route")] 
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        /// <summary>
        /// Verilen POI'lar için optimize edilmiş bir rota oluşturur.
        /// OptimizeOrder=true ise /trip, değilse /route OSRM endpoint'ini çağırır.
        /// </summary>
        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] RouteRequestDto request, CancellationToken ct)
        {
            // Model validasyon kontrolü
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Servis metodunu sadece bir kez çağırıyoruz.
            var result = await _routeService.GetOptimizedRouteAsync(request, ct);
            
            return Ok(result);
        }

        /// <summary>
        /// Oluşturulmuş bir rota için kullanıcıdan puan ve yorum alır.
        /// </summary>
        [HttpPost("{routeId:int}/feedback")]
        public IActionResult SubmitRouteFeedback(int routeId, [FromBody] CreateRouteFeedbackDto feedbackDto)
        {
            // Gelen verileri loglamak veya işlemek için burası doğru yer.
            // Console.WriteLine yerine _logger kullanmak daha iyi bir pratiktir.
            // _logger.LogInformation("Gelen Rota Puanı: {Rating}, Yorum: {Comment}", feedbackDto.Rating, feedbackDto.Comment);
            
            // TODO: Bu geri bildirimi veritabanına kaydetmek için bir servis metodu çağrılmalı.
            // Örnek: _routeService.SaveFeedbackAsync(routeId, feedbackDto);

            // Şimdilik, işlemin başarılı olduğuna dair bir mesaj dönüyoruz.
            return Ok(new { message = "Geri bildiriminiz için teşekkür ederiz!" });
        }
    }
}
