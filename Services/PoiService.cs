using GeziRotasi.API.Models;

namespace GeziRotasi.API.Services
{
    public class PoiService
    {
        // Veri listesi burada güvende.
        private static readonly List<Poi> _pois = new List<Poi>
        {
            new Poi { Id = 1, Name = "Topkapı Sarayı", /* ...diğer alanlar... */ },
            new Poi { Id = 2, Name = "Kapalıçarşı", /* ...diğer alanlar... */ }
        };

        // --- SENİN MEVCUT METOTLARIN (DOKUNULMADI) ---
        public List<Poi> GetAllPois() => _pois;

        public Poi? GetPoiById(int id) => _pois.FirstOrDefault(p => p.Id == id);

        public Poi CreatePoi(Poi newPoi)
        {
            newPoi.Id = _pois.Any() ? _pois.Max(p => p.Id) + 1 : 1;
            _pois.Add(newPoi);
            return newPoi;
        }

        public void UpdatePoi(Poi updatedPoi)
        {
            // ... (senin güncelleme mantığın burada) ...
        }

        public void DeletePoi(int id)
        {
            // ... (senin silme mantığın burada) ...
        }

        // --- SENA'NIN YENİ ÖZELLİĞİ İÇİN EKLENEN YENİ METOTLAR ---

        /// <summary>
        /// Verilen bir OSM ID'sinin sistemde zaten kayıtlı olup olmadığını kontrol eder.
        /// </summary>
        public bool IsOsmIdExists(long osmId)
        {
            return _pois.Any(p => p.ExternalApiId == osmId.ToString());
        }

        /// <summary>
        /// OpenStreetMap'ten gelen bir mekanı, sisteme yeni bir POI olarak ekler.
        /// </summary>
        public Poi ImportFromOsm(OsmElement osmElement)
        {
            // Gelen OSM verisini kendi Poi modelimize dönüştürüyoruz.
            // Bu, "mapping" (haritalama) işlemidir.
            var newPoi = new Poi
            {
                Name = osmElement.Tags.GetValueOrDefault("name", "İsimsiz Mekan"),
                Description = "OpenStreetMap üzerinden eklendi.",
                Latitude = osmElement.Lat,
                Longitude = osmElement.Lon,
                Category = osmElement.Tags.GetValueOrDefault("amenity", "Genel"),
                ExternalApiId = osmElement.Id.ToString(),
                OpeningHours = osmElement.Tags.GetValueOrDefault("opening_hours"),
                Website = osmElement.Tags.GetValueOrDefault("website"),
                PhoneNumber = osmElement.Tags.GetValueOrDefault("phone")
            };

            // ID atama ve listeye ekleme işlemini mevcut CreatePoi metodumuz yapıyor.
            // Bu, kod tekrarını önler.
            return CreatePoi(newPoi);
        }
    }
}
