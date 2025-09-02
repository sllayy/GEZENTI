using GeziRotasi.API.Data;
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteFilterController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RouteFilterController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Filtrelere göre rotaları listeler.
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterRoutes([FromBody] RouteFilterDto filter)
        {
            var query = _db.TravelRoutes.AsQueryable();

            if (filter.Categories != null && filter.Categories.Any())
            {
                query = query.Where(r => filter.Categories.Contains(r.Category));
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(r => r.AverageRating >= filter.MinRating.Value);
            }

            if (filter.MaxDistanceKm.HasValue)
            {
                query = query.Where(r => r.DistanceKm <= filter.MaxDistanceKm.Value);
            }

            if (!string.IsNullOrEmpty(filter.Duration))
            {
                query = query.Where(r => r.Duration == filter.Duration);
            }

            var routes = await query.ToListAsync();

            return Ok(routes);
        }

        /// <summary>
        /// Yeni rota ekler.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddRoute([FromBody] CreateRouteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var route = new TravelRoute
            {
                Category = dto.Category,
                AverageRating = dto.AverageRating,
                DistanceKm = dto.DistanceKm,
                Duration = dto.Duration
            };

            _db.TravelRoutes.Add(route);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Rota eklendi", route });
        }
    }
    }
