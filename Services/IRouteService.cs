using GeziRotasi.API.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeziRotasi.API.Services
{
    /// <summary>
    /// Rota optimizasyonu ile ilgili işlemleri tanımlayan sözleşmedir (interface).
    /// </summary>
    public interface IRouteService
    {
        /// <summary>
        /// Verilen koordinatlar için OSRM (Open Source Routing Machine) kullanarak
        /// optimize edilmiş bir rota hesaplar.
        /// </summary>
        /// <param name="request">Rota isteği için gerekli parametreleri içeren DTO.</param>
        /// <param name="ct">İsteğin iptal edilmesini sağlayan cancellation token.</param>
        /// <returns>Optimize edilmiş rotanın mesafe, süre ve geometri bilgilerini içeren bir DTO.</returns>
        Task<RouteResponseDto> GetOptimizedRouteAsync(RouteRequestDto request, CancellationToken ct = default);

        /// <summary>
        /// Belirtilen kullanıcı ID'sine ait tüm geçmiş rotaları listeler.
        /// </summary>
        /// <param name="userId">Kullanıcının ID'si.</param>
        /// <param name="ct">İsteğin iptal edilmesini sağlayan cancellation token.</param>
        /// <returns>Geçmiş rotaların bir listesini döner.</returns>
        Task<IEnumerable<PastRouteDto>> GetRoutesByUserIdAsync(int userId, CancellationToken ct = default);
    }
}
