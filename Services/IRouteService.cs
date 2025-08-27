using GeziRotasi.API.Dtos;
using GeziRotasi.API.Dtos;
using GeZentiRotasi.Api.Dtos;

namespace GeziRotasi.API.Services
namespace GeZentiRotasi.Api.Services
{
    public interface IRouteService
    {
        Task<object> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct); 
        
        Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default);
    }
}
