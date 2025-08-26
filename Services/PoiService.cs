using GeziRotasi.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace GeziRotasi.API.Services
{
    public class PoiService
    {
        // Gerçek veritabanı gelene kadar verileri burada, bellekte tutuyoruz.
        private static readonly List<Poi> _pois = new List<Poi>
        {
            new Poi 
            { 
                Id = 1, 
                Name = "Topkapı Sarayı", 
                Description = "Osmanlı İmparatorluğu'nun 600 yıllık tarihinin 400 yılı boyunca...", 
                Latitude = 41.0116, 
                Longitude = 28.9834, 
                Category = "Müze",
                ExternalApiId = null // Başlangıçta dış ID'si olmayabilir
            },
            new Poi 
            { 
                Id = 2, 
                Name = "Kapalıçarşı", 
                Description = "Dünyanın en eski ve en büyük kapalı çarşılarından biridir.", 
                Latitude = 41.0107, 
                Longitude = 28.9681, 
                Category = "Alışveriş",
                ExternalApiId = null
            }
        };

        /// <summary>
        /// Sistemdeki tüm Gezilecek Noktaları (POI) döndürür.
        /// </summary>
        public List<Poi> GetAllPois()
        {
            return _pois;
        }

        /// <summary>
        /// Verilen ID'ye sahip tek bir POI döndürür. Bulamazsa null döner.
        /// </summary>
        public Poi? GetPoiById(int id)
        {
            return _pois.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Sisteme yeni bir POI ekler ve eklenmiş halini geri döndürür.
        /// </summary>
        public Poi CreatePoi(Poi newPoi)
        {
            // Eğer listede eleman varsa en büyük ID'yi bulup 1 artır, yoksa ID'yi 1 yap.
            newPoi.Id = _pois.Any() ? _pois.Max(p => p.Id) + 1 : 1;
            _pois.Add(newPoi);
            return newPoi;
        }

        /// <summary>
        /// Mevcut bir POI'ı günceller.
        /// </summary>
        public void UpdatePoi(Poi updatedPoi)
        {
            var index = _pois.FindIndex(p => p.Id == updatedPoi.Id);
            if (index != -1)
            {
                // Bulunan indisteki eski POI'ı, tamamen yeni POI ile değiştiriyoruz.
                _pois[index] = updatedPoi;
            }
        }

        /// <summary>
        /// Verilen ID'ye sahip bir POI'ı sistemden siler.
        /// </summary>
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
            // OSM verisini kendi Poi modelimize dönüştürüyoruz (mapping).
            var newPoi = new Poi
            {
                // Yeni POI'ın özelliklerini OSM'den gelen verilerle dolduruyoruz.
                // Eğer bir tag (etiket) yoksa, varsayılan bir değer atıyoruz.
                Name = osmElement.Tags.GetValueOrDefault("name", "İsimsiz Mekan"),
                Description = "OpenStreetMap üzerinden eklendi.",
                Latitude = osmElement.Lat,
                Longitude = osmElement.Lon,
                Category = osmElement.Tags.GetValueOrDefault("amenity", "Genel"),
                ExternalApiId = osmElement.Id.ToString(),
                OpeningHours = osmElement.Tags.GetValueOrDefault("opening_hours"), // Yeni alan
                Website = osmElement.Tags.GetValueOrDefault("website"),             // Yeni alan
                PhoneNumber = osmElement.Tags.GetValueOrDefault("phone")            // Yeni alan
            };

            // ID atama ve listeye ekleme işini mevcut CreatePoi metodumuza yaptırıyoruz.
            return CreatePoi(newPoi);
        }
    }
}
