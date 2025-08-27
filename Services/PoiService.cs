using GeziRotasi.API.Data; // AppDbContext'i tanıyabilmek için gerekli
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore; // Entity Framework Core'u kullanabilmek için gerekli
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // async/await kullanabilmek için gerekli
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.IO;

namespace GeziRotasi.API.Services
{
    public class PoiService
    {
        // Artık statik bir liste yok! Onun yerine veritabanı temsilcimiz var.
        private readonly AppDbContext _context;

        // .NET, PoiService'i oluştururken, Program.cs'te kaydettiğimiz AppDbContext'i bize verecek.
        public PoiService(AppDbContext context)
        {
            _context = context;
        }

        // --- TÜM METOTLAR ARTIK ASENKRON VE VERİTABANI İLE ÇALIŞIYOR ---

        public async Task<List<Poi>> GetAllPoisAsync(string? category, string? searchTerm, string? sortBy, string? sortDirection, int pageNumber = 1, int pageSize = 10)
        {
            // Veritabanı tablosunu, üzerine filtreler ekleyebileceğimiz bir sorgu nesnesine dönüştürüyoruz.
            var query = _context.Pois.AsQueryable();

            // 1. FİLTRELEME BÖLÜMÜ
            // Kategoriye göre filtrele
            if (!string.IsNullOrWhiteSpace(category))
            {
                // Dışarıdan gelen metni (örn: "Müze") enum'a çevirmeye çalışıyoruz.
                if (Enum.TryParse<PoiCategory>(category, true, out var categoryEnum))
                {
                    // Eğer çevirme başarılıysa, sorguya "WHERE Category = ..." koşulunu ekliyoruz.
                    query = query.Where(p => p.Category == categoryEnum);
                }
            }

            // Arama terimine göre filtrele
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Sorguya "WHERE Name LIKE '%...%' OR Description LIKE '%...%'" koşulunu ekliyoruz.
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(searchTerm.ToLower())) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm.ToLower()))
                );
            }

            // 2. SIRALAMA BÖLÜMÜ
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                // Sıralama yönü "desc" (azalan) ise true, değilse false.
                bool isDescending = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

                // Hangi alana göre sıralanacağını belirliyoruz.
                if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                }
                else if (sortBy.Equals("category", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDescending ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category);
                }
                // Buraya gelecekte Id'ye veya başka alanlara göre sıralama da eklenebilir.
            }
            else
            {
                // VARSAYILAN SIRALAMA: Eğer hiçbir sıralama belirtilmemişse, isme göre A'dan Z'ye sırala.
                query = query.OrderBy(p => p.Name);
            }

            // 3. SAYFALAMA BÖLÜMÜ
            // Önce atlanacak kayıt sayısını hesaplıyoruz.
            var itemsToSkip = (pageNumber - 1) * pageSize;

            // Son olarak sorguya "OFFSET ... LIMIT ..." kurallarını ekleyip,
            // veritabanında çalıştırıyor ve sonucu bir liste olarak alıyoruz.
            return await query.Skip(itemsToSkip).Take(pageSize).ToListAsync();
        }

        public async Task<Poi?> GetPoiByIdAsync(int id)
        {
            // Veritabanında POI'ı bulurken...
            return await _context.Pois
                // ...ona bağlı olan Normal Çalışma Saatlerini de sorguya dahil et.
                .Include(p => p.WorkingHours)
                // ...ona bağlı olan Özel Gün Saatlerini de sorguya dahil et.
                .Include(p => p.SpecialDayHours)
                // Sadece ID'si eşleşen ilk kaydı bul.
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Poi> CreatePoiAsync(Poi newPoi)
        {
            // Yeni POI'ı veritabanı oturumuna ekler.
            _context.Pois.Add(newPoi);
            // Değişiklikleri veritabanına kaydeder. ID'si bu aşamada otomatik olarak atanır.
            await _context.SaveChangesAsync();
            return newPoi;
        }

        public async Task UpdatePoiAsync(int id, Poi updatedPoi)
        {
            // 1. Önce veritabanında güncellenecek olan asıl nesneyi buluyoruz.
            var poiToUpdate = await _context.Pois.FindAsync(id);

            // 2. Eğer bulunduysa, onun özelliklerini dışarıdan gelen yeni verilerle güncelliyoruz.
            if (poiToUpdate != null)
            {
                poiToUpdate.Name = updatedPoi.Name;
                poiToUpdate.Description = updatedPoi.Description;
                poiToUpdate.Latitude = updatedPoi.Latitude;
                poiToUpdate.Longitude = updatedPoi.Longitude;
                poiToUpdate.Category = updatedPoi.Category;
                poiToUpdate.OpeningHours = updatedPoi.OpeningHours;
                poiToUpdate.Website = updatedPoi.Website;
                poiToUpdate.PhoneNumber = updatedPoi.PhoneNumber;
                poiToUpdate.GirisUcreti = updatedPoi.GirisUcreti;
                poiToUpdate.OrtalamaZiyaretSuresi = updatedPoi.OrtalamaZiyaretSuresi;
                poiToUpdate.ImageUrl = updatedPoi.ImageUrl;

                // 3. Değişiklikleri veritabanına kaydediyoruz.
                await _context.SaveChangesAsync();
            }
            // Eğer bulunamazsa, bir şey yapmıyoruz. Hata yönetimi zaten Controller'da yapıldı.
        }

        public async Task DeletePoiAsync(int id)
        {
            // Önce silinecek POI'ı bulmamız gerekir.
            var poiToDelete = await _context.Pois.FindAsync(id);
            if (poiToDelete != null)
            {
                _context.Pois.Remove(poiToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsOsmIdExistsAsync(long osmId)
        {
            // Veritabanında bu ExternalApiId'ye sahip bir kayıt var mı diye kontrol eder.
            return await _context.Pois.AnyAsync(p => p.ExternalApiId == osmId.ToString());
        }

        public async Task<Poi> ImportFromOsmAsync(OsmElement osmElement)
        {
            // OSM'den gelen "amenity" string'ini alıyoruz.
            var categoryString = osmElement.Tags.GetValueOrDefault("amenity", "Genel");

            // Gelen string'i PoiCategory enum'ına dönüştürmeye çalışıyoruz.
            // Enum.TryParse, büyük/küçük harf duyarsız bir şekilde dönüşüm yapar.
            // Eğer başarılı bir dönüşüm yapamazsa, varsayılan olarak "Genel" kategorisini kullanır.
            Enum.TryParse<PoiCategory>(categoryString, true, out var poiCategory);

            var newPoi = new Poi
            {
                Name = osmElement.Tags.GetValueOrDefault("name", "İsimsiz Mekan"),
                Description = "OpenStreetMap üzerinden eklendi.",
                Latitude = osmElement.Lat,
                Longitude = osmElement.Lon,
                Category = poiCategory, // <-- DEĞİŞİKLİK BURADA
                ExternalApiId = osmElement.Id.ToString(),
                OpeningHours = osmElement.Tags.GetValueOrDefault("opening_hours"),
                Website = osmElement.Tags.GetValueOrDefault("website"),
                PhoneNumber = osmElement.Tags.GetValueOrDefault("phone")
            };

            return await CreatePoiAsync(newPoi);
        }
        // ÇALIŞMA SAATİ METOTLARI

        /// Belirli bir POI'ın tüm çalışma saatlerini getirir.
        /// <param name="poiId">Çalışma saatleri istenen POI'ın ID'si</param>
        public async Task<List<WorkingHour>> GetWorkingHoursForPoiAsync(int poiId)
        {
            // Veritabanındaki WorkingHours tablosuna git,
            // PoiId'si bizim verdiğimiz poiId ile eşleşen tüm kayıtları bul
            // ve bunları bir liste olarak döndür.
            return await _context.WorkingHours
                .Where(wh => wh.PoiId == poiId)
                .ToListAsync();
        }

        /// Bir POI için yeni bir çalışma saati kaydı ekler.
        /// <param name="workingHour">Eklenecek çalışma saati bilgisi</param>
        public async Task<WorkingHour> AddWorkingHourAsync(WorkingHour workingHour)
        {
            // Gelen yeni çalışma saati nesnesini veritabanı oturumuna ekle.
            _context.WorkingHours.Add(workingHour);
            // Değişiklikleri veritabanına kaydet.
            await _context.SaveChangesAsync();
            return workingHour;
        }

        // ÖZEL GÜN SAATLERİ METOTLARI 

        /// Belirli bir POI'ın tüm özel gün kayıtlarını getirir.
        public async Task<List<SpecialDayHour>> GetSpecialDayHoursForPoiAsync(int poiId)
        {
            // Veritabanındaki SpecialDayHours tablosuna git,
            // PoiId'si bizim verdiğimiz poiId ile eşleşen tüm kayıtları bul
            // ve bunları tarihe göre sıralı bir şekilde döndür.
            return await _context.SpecialDayHours
                .Where(sdh => sdh.PoiId == poiId)
                .OrderBy(sdh => sdh.Date)
                .ToListAsync();
        }

        /// Bir POI için yeni bir özel gün kaydı ekler.
        public async Task<SpecialDayHour> AddSpecialDayHourAsync(SpecialDayHour specialDayHour)
        {
            _context.SpecialDayHours.Add(specialDayHour);
            await _context.SaveChangesAsync();
            return specialDayHour;
        }

        // GEOJSON METODU
        /// Verilen bir POI listesini GeoJSON FeatureCollection formatına dönüştürür.
        public string ConvertPoisToGeoJson(List<Poi> pois)
        {
            var features = new List<Feature>();

            foreach (var poi in pois)
            {
                // Her bir POI için bir coğrafi "Nokta" (Point) oluşturuyoruz.
                var point = new Point(poi.Longitude, poi.Latitude);

                // Bu noktaya, POI'ın diğer tüm bilgilerini "Attributes" (Özellikler) olarak ekliyoruz.
                var attributes = new AttributesTable();
                attributes.Add("id", poi.Id);
                attributes.Add("name", poi.Name);
                attributes.Add("description", poi.Description);
                attributes.Add("category", poi.Category.ToString()); // Enum'ı string'e çeviriyoruz
                attributes.Add("imageUrl", poi.ImageUrl);
                // ... buraya haritada göstermek istediğin diğer tüm alanları ekleyebilirsin ...

                // Noktayı ve özellikleri birleştirerek bir "Feature" (Coğrafi Öğe) oluşturuyoruz.
                features.Add(new Feature(point, attributes));
            }

            // Önce boş bir FeatureCollection oluşturuyoruz.
            var featureCollection = new FeatureCollection();
            // Sonra, daha önce oluşturduğumuz tüm "feature"ları bu koleksiyona ekliyoruz.
            features.ForEach(f => featureCollection.Add(f));
            // Bu koleksiyonu bir GeoJSON metnine çeviriyoruz.
            
            // Bu, NetTopologySuite'in KENDİ, System.Text.Json ile uyumlu
            // tercümanını kullanarak FeatureCollection'ı doğrudan bir string'e çevirir.
            return new GeoJsonWriter().Write(featureCollection);
        }
    }
}