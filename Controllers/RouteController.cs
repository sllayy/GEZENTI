using GeZenti.Api.Dtos;
using GeZenti.Api.Services;
using Microsoft.AspNetCore.Mvc;
//conflict test
namespace GeZenti.Api.Controllers
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
            var result = await _routeService.GetOptimizedRouteAsync(request, ct);
            return Ok(result);
        }
    }
}
