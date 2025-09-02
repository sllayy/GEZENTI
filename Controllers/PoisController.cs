#nullable enable
using Microsoft.AspNetCore.Mvc;
using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using GeziRotasi.API.Entities;

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
        [AllowAnonymous]
        public async Task<IActionResult> GetPois(
            [FromQuery] string? category,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortBy = "populer",
            [FromQuery(Name = "sort")] string? sortDirection = "desc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string format = "json")
        {
            try
            {
                var pois = await _poiService.GetAllPoisAsync(category, searchTerm, sortBy, sortDirection, pageNumber, pageSize);

                if (format.Equals("geojson", StringComparison.OrdinalIgnoreCase))
                {
                    var geoJson = _poiService.ConvertPoisToGeoJson(pois);
                    return Content(geoJson, "application/geo+json");
                }

                return Ok(pois);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPois endpoint'inde hata oluştu.");
                return StatusCode(500, "POI'ler alınırken sunucuda bir hata oluştu.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPoiById(int id)
        {
            var poi = await _poiService.GetPoiByIdAsync(id);
            if (poi == null)
            {
                _logger.LogWarning("ID {PoiId} ile POI bulunamadı.", id);
                return NotFound();
            }
            return Ok(poi);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoi([FromBody] Poi newPoi)
        {
            try
            {
                var createdPoi = await _poiService.CreatePoiAsync(newPoi);
                _logger.LogInformation("Yeni POI oluşturuldu: {PoiId}", createdPoi.Id);
                return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreatePoi endpoint'inde hata oluştu.");
                return StatusCode(500, "POI oluşturulurken sunucuda bir hata oluştu.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePoi(int id, [FromBody] Poi updatedPoi)
        {
            if (id != updatedPoi.Id)
            {
                return BadRequest("URL ID'si ile gövde ID'si eşleşmiyor.");
            }

            var existingPoi = await _poiService.GetPoiByIdAsync(id);
            if (existingPoi == null)
            {
                _logger.LogWarning("Güncellenecek ID {PoiId} ile POI bulunamadı.", id);
                return NotFound();
            }

            try
            {
                await _poiService.UpdatePoiAsync(id, updatedPoi);
                _logger.LogInformation("POI güncellendi: {PoiId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePoi endpoint'inde hata oluştu.");
                return StatusCode(500, "POI güncellenirken sunucuda bir hata oluştu.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePoi(int id)
        {
            var existingPoi = await _poiService.GetPoiByIdAsync(id);
            if (existingPoi == null)
            {
                _logger.LogWarning("Silinecek ID {PoiId} ile POI bulunamadı.", id);
                return NotFound();
            }

            try
            {
                await _poiService.DeletePoiAsync(id);
                _logger.LogInformation("POI silindi: {PoiId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeletePoi endpoint'inde hata oluştu.");
                return StatusCode(500, "POI silinirken sunucuda bir hata oluştu.");
            }
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
                    _logger.LogInformation("Yakın çevrede mekan bulunamadı. Lat: {Lat}, Lon: {Lon}, Type: {Type}", lat, lon, type);
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
                    _logger.LogWarning("OSM'de {OsmId} ID'li mekan bulunamadı.", osmId);
                    return NotFound($"OSM'de {osmId} ID'li mekan bulunamadı.");
                }

                var createdPoi = await _poiService.ImportFromOsmAsync(osmElement);
                _logger.LogInformation("OSM'den yeni POI içe aktarıldı: {PoiId}", createdPoi.Id);
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
