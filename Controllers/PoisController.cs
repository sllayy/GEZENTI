using Microsoft.AspNetCore.Mvc;
using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using System.Threading.Tasks; // async/await için gerekli
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

        // Constructor aynı kalıyor, değişiklik yok.
        public PoisController(PoiService poiService, OsmService osmService, ILogger<PoisController> logger)
        {
            _poiService = poiService;
            _osmService = osmService;
            _logger = logger;
        }

        // --- TÜM METOTLAR ARTIK ASENKRON ---

        [HttpGet]
        public async Task<IActionResult> GetPois([FromQuery] string? category,
                                     [FromQuery] string? searchTerm,
                                     [FromQuery] string? sortBy,
                                     [FromQuery(Name = "sort")] string? sortDirection,
                                     [FromQuery] int pageNumber = 1,
                                     [FromQuery] int pageSize = 10,
                                     [FromQuery] string? format = "json") // YENİ FORMAT PARAMETRESİ
        public IActionResult GetPois([FromQuery] string? category, [FromQuery] string? searchTerm, [FromQuery] string? sortBy, [FromQuery(Name = "sort")] string? sortDirection, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Önce, her zamanki gibi verileri veritabanından alıyoruz.
            var pois = await _poiService.GetAllPoisAsync(category, searchTerm, sortBy, sortDirection, pageNumber, pageSize);

            // Şimdi, istenen formata göre cevabı hazırlıyoruz.
            if (format?.Equals("geojson", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                // Eğer format "geojson" ise, GeoJSON'a çeviriyoruz.
                var geoJson = _poiService.ConvertPoisToGeoJson(pois);
                // Cevabın türünün "application/geo+json" olduğunu belirtiyoruz.
                return Content(geoJson, "application/geo+json");
            }
            else
            {
                // Değilse, her zamanki gibi normal JSON olarak döndürüyoruz.
            var pois = _poiService.GetAllPois(category, searchTerm, sortBy, sortDirection, pageNumber, pageSize);
            return Ok(pois);
        }
        }

        [HttpGet("{id}")]
        public IActionResult GetPoiById(int id)
        public async Task<IActionResult> GetPoiById(int id)
        {
            var poi = _poiService.GetPoiById(id);
            var poi = await _poiService.GetPoiByIdAsync(id);
            if (poi == null) return NotFound();
            return Ok(poi);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoi([FromBody] Poi newPoi)
        public IActionResult CreatePoi([FromBody] Poi newPoi)
        {
            var createdPoi = await _poiService.CreatePoiAsync(newPoi);
            var createdPoi = _poiService.CreatePoi(newPoi);
            return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePoi(int id, [FromBody] Poi updatedPoi)
        public async Task<IActionResult> UpdatePoi(int id, [FromBody] Poi updatedPoi)
        {
            if (id != updatedPoi.Id) return BadRequest("URL ID'si ile gövde ID'si eşleşmiyor.");
            var existingPoi = _poiService.GetPoiById(id);

            var existingPoi = await _poiService.GetPoiByIdAsync(id);
            if (existingPoi == null) return NotFound();
            _poiService.UpdatePoi(updatedPoi);

            // Artık servise hem ID'yi hem de yeni veriyi gönderiyoruz.
            await _poiService.UpdatePoiAsync(id, updatedPoi);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePoi(int id)
        public async Task<IActionResult> DeletePoi(int id)
        {
            var existingPoi = await _poiService.GetPoiByIdAsync(id);
            var existingPoi = _poiService.GetPoiById(id);
            if (existingPoi == null) return NotFound();
            _poiService.DeletePoi(id);

            await _poiService.DeletePoiAsync(id);
            return NoContent();
        }

        // Bu metotlar zaten asenkrondu, sadece içlerindeki servis çağrılarını güncelliyoruz.
        [HttpGet("search-nearby")]
        public async Task<IActionResult> SearchNearbyPlaces([FromQuery] double lat, [FromQuery] double lon, [FromQuery] string type, [FromQuery] int radius = 1000)
        {
            try
            {
                // İşin tamamı OsmService'e devrediliyor.
                var places = await _osmService.GetPlacesAsync(lat, lon, type, radius);

                if (places == null || !places.Elements.Any())
                {
                    return NotFound("Belirtilen kriterlere uygun mekan bulunamadı.");
                }
                return Ok(places);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchNearbyPlaces endpoint'inde bir hata oluştu.");
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
                if (_poiService.IsOsmIdExists(osmId))
                {
                    return Conflict("Bu mekan zaten sisteme eklenmiş.");
                }

                var osmElement = await _osmService.GetPlaceDetailsAsync(osmId);
                if (osmElement == null)
                {
                    return NotFound($"OSM'de {osmId} ID'li mekan bulunamadı.");
                }

                var createdPoi = await _poiService.ImportFromOsmAsync(osmElement);

                var createdPoi = _poiService.ImportFromOsm(osmElement);
                return CreatedAtAction(nameof(GetPoiById), new { id = createdPoi.Id }, createdPoi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportFromOsm endpoint'inde hata oluştu.");
                return StatusCode(500, "Veri işlenirken bir hata oluştu.");
            }
        }
        // --- YENİ EKLENEN ÇALIŞMA SAATİ ENDPOINT'LERİ ---

        /// Belirli bir POI'ın çalışma saatlerini listeler.
        /// URL Örneği: GET /api/pois/5/workinghours
        [HttpGet("{id:int}/workinghours")]
        public async Task<IActionResult> GetWorkingHours(int id)
        {
            var workingHours = await _poiService.GetWorkingHoursForPoiAsync(id);

            // Eğer o POI için hiç çalışma saati eklenmemişse boş bir liste döner, bu bir hata değildir.
            return Ok(workingHours);
        }

        /// Belirli bir POI'a yeni bir çalışma saati ekler.
        /// URL Örneği: POST /api/pois/5/workinghours
        [HttpPost("{id:int}/workinghours")]
        public async Task<IActionResult> AddWorkingHour(int id, [FromBody] WorkingHour workingHour)
        {
            // Güvenlik kontrolü: URL'deki POI ID'si ile gönderilen verinin içindeki
            // POI ID'sinin aynı olduğundan emin oluyoruz.
            if (id != workingHour.PoiId)
            {
                return BadRequest("URL ID'si ile gövdedeki POI ID'si eşleşmiyor.");
            }

            // Servis aracılığıyla veriyi veritabanına ekliyoruz.
            var createdWorkingHour = await _poiService.AddWorkingHourAsync(workingHour);

            // Başarılı bir şekilde oluşturulduğunu belirten 201 Created cevabı dönüyoruz.
            // Bu, en iyi pratiktir.
            return CreatedAtAction(nameof(GetWorkingHours), new { id = createdWorkingHour.PoiId }, createdWorkingHour);
        }
        //ÖZEL GÜN SAATLERİ ENDPOINT'LERİ 

        /// Belirli bir POI'ın özel gün çalışma saatlerini listeler.
        /// URL Örneği: GET /api/pois/5/specialdayhours
        [HttpGet("{id:int}/specialdayhours")]
        public async Task<IActionResult> GetSpecialDayHours(int id)
        {
            var specialHours = await _poiService.GetSpecialDayHoursForPoiAsync(id);
            return Ok(specialHours);
        }

        /// Belirli bir POI'a yeni bir özel gün çalışma saati ekler.
        /// URL Örneği: POST /api/pois/5/specialdayhours
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