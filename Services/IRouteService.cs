using GeziRotasi.API.Dtos;

namespace GeziRotasi.API.Services
{
    public interface IRouteService
    {
        Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default);
    }
}
