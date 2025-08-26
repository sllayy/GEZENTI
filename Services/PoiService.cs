using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;

namespace GeziRotasi.API.Services
{
    public class PoiService
    {
        // VERİ LİSTESİ ARTIK BURADA GÜVENDE
        private static readonly List<Poi> _pois = new List<Poi>
        {
            new Poi { Id = 1, Name = "Topkapı Sarayı", Description = "..." },
            new Poi { Id = 2, Name = "Kapalıçarşı", Description = "..." }
        };

        // Tüm POI'ları getiren metot
        public List<Poi> GetAllPois()
        {
            return _pois;
        }

        // ID'ye göre tek bir POI getiren metot
        public Poi? GetPoiById(int id)
        {
            return _pois.FirstOrDefault(p => p.Id == id);
        }

        // Yeni bir POI oluşturan metot
        public Poi CreatePoi(Poi newPoi)
        {
            var newId = _pois.Any() ? _pois.Max(p => p.Id) + 1 : 1;
            newPoi.Id = newId;
            _pois.Add(newPoi);
            return newPoi;
        }

        // Bir POI'ı güncelleyen metot
        public void UpdatePoi(Poi updatedPoi)
        {
            var index = _pois.FindIndex(p => p.Id == updatedPoi.Id);
            if (index != -1)
            {
                _pois[index] = updatedPoi;
            }
        }

        // Bir POI'ı silen metot
        public void DeletePoi(int id)
        {
            var poiToDelete = _pois.FirstOrDefault(p => p.Id == id);
            if (poiToDelete != null)
            {
                _pois.Remove(poiToDelete);
            }
        }
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

            // ID atama ve listeye ekleme işini mevcut CreatePoi metodumuza yaptırıyoruz.
            return CreatePoi(newPoi);
        }
    }
}
