#nullable enable
using GeziRotasi.API.Dtos;

namespace GeziRotasi.API.Services
{
    public interface IMapRouteService
    {
        // Sadece rota (başlangıç → bitiş)
        Task<RouteResponseDto> GetRouteAsync(
            RouteRequestDto request,
            CancellationToken ct = default
        );

        // Rota + rota üzerindeki POI’ler
        Task<RouteResponseDto> GetRouteWithPoisAsync(
            RouteRequestDto request,
            CancellationToken ct = default
        );

        // Rota kaydetme
        Task<int> SaveRouteAsync(
            RouteResponseDto dto,
            int userId,
            double[] start,
            double[]? end,
            CancellationToken ct = default
        );
    }
}
