using GeziRotasi.API.Dtos;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapRouteController : ControllerBase
    {
        private readonly IMapRouteService _mapRouteService;

        public MapRouteController(IMapRouteService mapRouteService)
        {
            _mapRouteService = mapRouteService;
        }

        [HttpPost("with-pois")]
        public async Task<ActionResult<RouteResponseDto>> GetRouteWithPois([FromBody] RouteRequestDto request, CancellationToken ct)
        {
            var result = await _mapRouteService.GetRouteWithPoisAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<ActionResult<int>> SaveRoute([FromBody] RouteSaveRequest request, CancellationToken ct)
        {
            var routeId = await _mapRouteService.SaveRouteAsync(
                request.Route,
                request.UserId,
                request.Start,
                request.End,
                ct
            );

            return Ok(routeId);
        }
    }
}
