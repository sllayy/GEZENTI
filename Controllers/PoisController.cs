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

        [HttpGet]
        public IActionResult GetPois([FromQuery] string? category, [FromQuery] string? searchTerm, [FromQuery] string? sortBy, [FromQuery(Name = "sort")] string? sortDirection, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pois = _poiService.GetAllPois(category, searchTerm, sortBy, sortDirection, pageNumber, pageSize);
            return Ok(pois);
        }

        [HttpGet("{id}")]
        public IActionResult GetPoiById(int id)
        {
            var poi = _poiService.GetPoiById(id);
            if (poi == null) return NotFound();
            return Ok(poi);
        }

        [HttpPost]
        public IActionResult CreatePoi([FromBody] Poi newPoi)
        {
            var createdPoi = _poiService.CreatePoi(newPoi);
            return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePoi(int id, [FromBody] Poi updatedPoi)
        {
            if (id != updatedPoi.Id) return BadRequest("URL ID'si ile gövde ID'si eşleşmiyor.");
            var existingPoi = _poiService.GetPoiById(id);
            if (existingPoi == null) return NotFound();
            _poiService.UpdatePoi(updatedPoi);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePoi(int id)
        {
            var existingPoi = _poiService.GetPoiById(id);
            if (existingPoi == null) return NotFound();
            _poiService.DeletePoi(id);
            return NoContent();
        }

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
                if (_poiService.IsOsmIdExists(osmId))
                {
                    return Conflict("Bu mekan zaten sisteme eklenmiş.");
                }
                var osmElement = await _osmService.GetPlaceDetailsAsync(osmId);
                if (osmElement == null)
                {
                    return NotFound($"OSM'de {osmId} ID'li mekan bulunamadı.");
                }
                var createdPoi = _poiService.ImportFromOsm(osmElement);
                return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportFromOsm endpoint'inde hata oluştu.");
                return StatusCode(500, "Veri işlenirken bir hata oluştu.");
            }
        }
    }
}