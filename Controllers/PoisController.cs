using Microsoft.AspNetCore.Mvc;
using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoisController : ControllerBase
    {
        // Artık Controller'da veri listesi yok!
        private readonly PoiService _poiService; // Sadece PoiService'i tanıyor
        private readonly OsmService _osmService;

        // Constructor artık iki servisi de istiyor
        public PoisController(PoiService poiService, OsmService osmService)
        {
            _poiService = poiService;
            _osmService = osmService;
        }

        // Tüm metotlar artık çok daha kısa ve temiz!
        [HttpGet]
        public IActionResult GetPois()
        {
            return Ok(_poiService.GetAllPois());
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

        // --- SENA'NIN KODUNUN ENTEGRE EDİLDİĞİ BÖLÜM (Değişiklik Yok) ---
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
                Console.WriteLine($"Hata: {ex.Message}");
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
                Console.WriteLine($"Hata: {ex.Message}");
                return StatusCode(500, "Veri işlenirken bir hata oluştu.");
            }
        }
    }
}
