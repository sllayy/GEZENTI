using GeziRotasi.API.Dtos;
using GeziRotasi.API.Dtos;

namespace GeziRotasi.API.Services
{
    public interface IRouteService
    {
        Task<object> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct); 
        
    }
}
