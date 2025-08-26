using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoisController : ControllerBase
    {
        private readonly PoiService _poiService;
        private readonly OsmService _osmService;

        public PoisController(PoiService poiService, OsmService osmService)
        {
            _poiService = poiService;
            _osmService = osmService;
        }

        // --- SENİN TEMİZ CRUD METOTLARIN (DEĞİŞİKLİK YOK) ---
        [HttpGet]
        public IActionResult GetPois() => Ok(_poiService.GetAllPois());

        [HttpGet("{id}")]
        public IActionResult GetPoiById(int id)
        {
            // ... (senin kodun) ...
        }

        [HttpPost]
        public IActionResult CreatePoi([FromBody] Poi newPoi)
        {
            // ... (senin kodun) ...
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePoi(int id, [FromBody] Poi updatedPoi)
        {
            // ... (senin kodun) ...
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePoi(int id)
        {
            // ... (senin kodun) ...
        }
        
        // --- SENA'NIN MEVCUT ÖZELLİĞİ (DEĞİŞİKLİK YOK) ---
        [HttpGet("search-nearby")]
        public async Task<IActionResult> SearchNearbyPlaces([FromQuery] double lat, /*...*/)
        {
            // ... (senin kodunla aynı) ...
        }

        // --- SENA'NIN YENİ ÖZELLİĞİ (YENİ YAPIYA UYGUN HALE GETİRİLDİ) ---
        [HttpPost("import-from-osm")]
        public async Task<IActionResult> ImportFromOsm([FromQuery] long osmId)
        {
            try
            {
                // Kontrolü artık PoiService yapıyor. Controller'ın işi değil.
                if (_poiService.IsOsmIdExists(osmId))
                {
                    return Conflict("Bu mekan zaten sisteme eklenmiş.");
                }

                // Detayları OsmService'ten alıyoruz.
                var osmElement = await _osmService.GetPlaceDetailsAsync(osmId);
                if (osmElement == null)
                {
                    return NotFound($"OSM'de {osmId} ID'li mekan bulunamadı.");
                }

                // Tüm dönüştürme ve ekleme işini PoiService'e devrediyoruz.
                var createdPoi = _poiService.ImportFromOsm(osmElement);

                // Başarılı cevabı dönüyoruz.
                return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
            }
            catch (Exception ex)
            {
                // Loglama ve hata yönetimi
                return StatusCode(500, "Veri işlenirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
