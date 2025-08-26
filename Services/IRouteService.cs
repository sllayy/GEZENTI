using GeZenti.Api.Dtos;

namespace GeZenti.Api.Services
{
    public interface IRouteService
    {
        Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default);
    }
}
