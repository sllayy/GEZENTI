using GeziRotasi.API.Data;
using GeziRotasi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/travelroutes")]
    public class TravelRoutesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TravelRoutesController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Tüm travel route kayıtlarını listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var routes = await _db.TravelRoutes.ToListAsync();
            return Ok(routes);
        }

        /// <summary>
        /// ID’ye göre travel route getirir.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _db.TravelRoutes.FindAsync(id);
            if (route == null)
                return NotFound();

            return Ok(route);
        }

        /// <summary>
        /// Yeni travel route ekler.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TravelRoute dto)
        {
            _db.TravelRoutes.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Travel route günceller.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TravelRoute dto)
        {
            var route = await _db.TravelRoutes.FindAsync(id);
            if (route == null)
                return NotFound();

            route.Category = dto.Category;
            route.AverageRating = dto.AverageRating;
            route.DistanceKm = dto.DistanceKm;
            route.Duration = dto.Duration;

            await _db.SaveChangesAsync();
            return Ok(route);
        }

        /// <summary>
        /// Travel route siler.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var route = await _db.TravelRoutes.FindAsync(id);
            if (route == null)
                return NotFound();

            _db.TravelRoutes.Remove(route);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
