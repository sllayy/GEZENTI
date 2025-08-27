using Microsoft.AspNetCore.Mvc;
using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoisController : ControllerBase
    {
        private readonly PoiService _poiService;
        private readonly OsmService _osmService;
        private readonly ILogger<PoisController> _logger;

        public PoisController(PoiService poiService, OsmService osmService, ILogger<PoisController> logger)
        {
            _poiService = poiService;
            _osmService = osmService;
            _logger = logger;
        }

        // --- TEMEL POI ENDPOINT'LERİ ---

        [HttpGet]
        public async Task<IActionResult> GetPois(
            [FromQuery] string? category, [FromQuery] string? searchTerm, 
            [FromQuery] string? sortBy, [FromQuery(Name = "sort")] string? sortDirection, 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? format = "json")
        {
            var pois = await _poiService.GetAllPoisAsync(category, searchTerm, sortBy, sortDirection, pageNumber, pageSize);

            if (format?.Equals("geojson", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var geoJson = _poiService.ConvertPoisToGeoJson(pois);
                return Content(geoJson, "application/geo+json");
            }
            
            return Ok(pois);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPoiById(int id)
        {
            var poi = await _poiService.GetPoiByIdAsync(id);
            if (poi == null) return NotFound();
            return Ok(poi);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoi([FromBody] Poi newPoi)
        {
            var createdPoi = await _poiService.CreatePoiAsync(newPoi);
            return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePoi(int id, [FromBody] Poi updatedPoi)
        {
            if (id != updatedPoi.Id) return BadRequest("URL ID'si ile gövde ID'si eşleşmiyor.");
            
            var existingPoi = await _poiService.GetPoiByIdAsync(id);
            if (existingPoi == null) return NotFound();
            
            await _poiService.UpdatePoiAsync(id, updatedPoi);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoi(int id)
        {
            var existingPoi = await _poiService.GetPoiByIdAsync(id);
            if (existingPoi == null) return NotFound();
            
            await _poiService.DeletePoiAsync(id);
            return NoContent();
        }

        // --- OSM ENTEGRASYON ENDPOINT'LERİ ---

        [HttpGet("search-nearby")]
        public async Task<IActionResult> SearchNearbyPlaces([FromQuery] double lat, [FromQuery] double lon, [FromQuery] string type, [FromQuery] int radius = 1000)
        {
            try
            {
                var places = await _osmService.GetPlacesAsync(lat, lon, type, radius);
                if (places == null || !places.Elements.Any())
                {
                    return NotFound("Belirtilen kriterlere uygun mekan bulunamadı.");
                }
                return Ok(places);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchNearbyPlaces endpoint'inde hata oluştu.");
                return StatusCode(500, "Mekanlar aranırken sunucuda bir hata oluştu.");
            }
        }

        [HttpPost("import-from-osm")]
        public async Task<IActionResult> ImportFromOsm([FromQuery] long osmId)
        {
            try
            {
                if (await _poiService.IsOsmIdExistsAsync(osmId))
                {
                    return Conflict("Bu mekan zaten sisteme eklenmiş.");
                }
                var osmElement = await _osmService.GetPlaceDetailsAsync(osmId);
                if (osmElement == null)
                {
                    return NotFound($"OSM'de {osmId} ID'li mekan bulunamadı.");
                }
                var createdPoi = await _poiService.ImportFromOsmAsync(osmElement);
                return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportFromOsm endpoint'inde hata oluştu.");
                return StatusCode(500, "Veri işlenirken bir hata oluştu.");
            }
        }
        
        // --- ÇALIŞMA SAATLERİ ENDPOINT'LERİ ---

        [HttpGet("{id:int}/workinghours")]
        public async Task<IActionResult> GetWorkingHours(int id)
        {
            var workingHours = await _poiService.GetWorkingHoursForPoiAsync(id);
            return Ok(workingHours);
        }

        [HttpPost("{id:int}/workinghours")]
        public async Task<IActionResult> AddWorkingHour(int id, [FromBody] WorkingHour workingHour)
        {
            if (id != workingHour.PoiId)
            {
                return BadRequest("URL ID'si ile gövdedeki POI ID'si eşleşmiyor.");
            }
            var createdWorkingHour = await _poiService.AddWorkingHourAsync(workingHour);
            return CreatedAtAction(nameof(GetWorkingHours), new { id = createdWorkingHour.PoiId }, createdWorkingHour);
        }

        // --- ÖZEL GÜN SAATLERİ ENDPOINT'LERİ ---

        [HttpGet("{id:int}/specialdayhours")]
        public async Task<IActionResult> GetSpecialDayHours(int id)
        {
            var specialHours = await _poiService.GetSpecialDayHoursForPoiAsync(id);
            return Ok(specialHours);
        }

        [HttpPost("{id:int}/specialdayhours")]
        public async Task<IActionResult> AddSpecialDayHour(int id, [FromBody] SpecialDayHour specialDayHour)
        {
            if (id != specialDayHour.PoiId)
            {
                return BadRequest("URL ID'si ile gövdedeki POI ID'si eşleşmiyor.");
            }
            var createdSpecialHour = await _poiService.AddSpecialDayHourAsync(specialDayHour);
            return CreatedAtAction(nameof(GetSpecialDayHours), new { id = createdSpecialHour.PoiId }, createdSpecialHour);
        }
    }
}
