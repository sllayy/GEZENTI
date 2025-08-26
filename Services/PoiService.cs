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
    }
}