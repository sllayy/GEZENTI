using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoisController : ControllerBase
    {
        // ESKİ YERİNE GERİ GETİRDİK: Sahte veri listesi artık yine Controller'ın içinde.
        private static readonly List<Poi> _pois = new List<Poi>
        {
            new Poi
            {
                Id = 1,
                Name = "Topkapı Sarayı",
                Description = "Osmanlı İmparatorluğu'nun...", 
                // ... diğer alanlar ...
            },
            new Poi
            {
                Id = 2,
                Name = "Kapalıçarşı",
                Description = "Dünyanın en eski ve en büyük...", 
                // ... diğer alanlar ...
            }
        };

        private readonly OsmService _osmService; // Sadece Sena'nın servisini alıyoruz.

        // Constructor'ı sadece OsmService'i alacak şekilde güncelledik.
        public PoisController(OsmService osmService)
        {
            _osmService = osmService;
        }

        // --- SENİN YAZDIĞIN CRUD ENDPOINT'LERİ (Artık Doğru Çalışacak) ---

        [HttpGet]
        public IActionResult GetPois()
        {
            return Ok(_pois);
        }

        [HttpGet("{id}")]
        public IActionResult GetPoiById(int id)
        {
            var poi = _pois.FirstOrDefault(p => p.Id == id);
            if (poi == null) return NotFound();
            return Ok(poi);
        }

        [HttpPost]
        public IActionResult CreatePoi([FromBody] Poi newPoi)
        {
            var newId = _pois.Max(p => p.Id) + 1;
            newPoi.Id = newId;
            _pois.Add(newPoi);
            return CreatedAtAction(nameof(GetPoiById), new { id = newId }, newPoi);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePoi(int id, [FromBody] Poi updatedPoi)
        {
            var poiToUpdate = _pois.FirstOrDefault(p => p.Id == id);
            if (poiToUpdate == null) return NotFound();

            // ... güncelleme mantığı ...
            poiToUpdate.Name = updatedPoi.Name;
            poiToUpdate.Description = updatedPoi.Description;
            // ... diğer alanlar ...

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePoi(int id)
        {
            var poiToDelete = _pois.FirstOrDefault(p => p.Id == id);
            if (poiToDelete == null) return NotFound();

            _pois.Remove(poiToDelete);
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
    }
}