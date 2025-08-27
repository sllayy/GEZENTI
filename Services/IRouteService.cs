using GeZentiRotasi.Api.Dtos;

namespace GeZentiRotasi.Api.Services
{
    public interface IRouteService
    {
        Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default);
    }
}
