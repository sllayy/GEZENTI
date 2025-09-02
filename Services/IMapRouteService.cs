using GeziRotasi.API.Dtos;

namespace GeziRotasi.API.Services
{
    public interface IMapRouteService
    {
        Task<RouteResponseDto> GetRouteWithPoisAsync(
            RouteRequestDto request, 
            CancellationToken ct = default
         );

        Task<int> SaveRouteAsync(
            RouteResponseDto dto,
            int userId,
            double[] start,
            double[]? end,
            CancellationToken ct = default
        );
    }
}
