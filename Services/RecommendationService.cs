using GeziRotasi.API.Data;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GeziRotasi.API.Services
{
    public class RecommendationService
    {
        private readonly AppDbContext _db;

        private readonly Dictionary<string, PoiCategory> _themeMap = new()
        {
            { "Tarih", PoiCategory.Tarih },
            { "Müze", PoiCategory.Müze },
            { "Sanat", PoiCategory.Sanat },
            { "Yemek", PoiCategory.Yemek },
            { "Alışveriş", PoiCategory.Alışveriş },
            { "Doğa", PoiCategory.Doğa },
            { "Eğlence", PoiCategory.Eğlence },
            { "Müzik", PoiCategory.Müzik },
            { "Sahil", PoiCategory.Sahil },
            { "Park", PoiCategory.Park },
            { "Karayolu", PoiCategory.Karayolu },
            { "Önemli", PoiCategory.Önemli },
        };

        public RecommendationService(AppDbContext db) => _db = db;

        /// <summary>
        /// Kullanıcı ID’sine göre DB tercihlerinden ve konumdan öneri üretir (sadece id listesi).
        /// Saat/review verisi yoksa KAPSAYICI davranır. Yarıçapta sonuç yoksa 30→50→75→100 km dener.
        /// </summary>
        public async Task<object> GetRecommendedPoiIdsByUserIdAsync(
            int userId,
            double userLat, double userLon,
            double radiusKm,
            CancellationToken ct = default)
        {
            // Kullanıcı konumu aralık kontrolü  
            if (!(userLat is >= -90 and <= 90) || !(userLon is >= -180 and <= 180))
            {
                return new
                {
                    poiIds = new List<int>(),
                    totalFound = 0,
                    radiusKmUsed = 0,
                    error = "Geçersiz kullanıcı konumu (lat/lon aralık dışı)."
                };
            }

            //  Kullanıcı tercihleri
            var prefs = await _db.UserPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId, ct);

            //  SQL'e çevrilebilir POI filtresi
            var poisQuery = _db.Pois
                .Include(p => p.Reviews)
                .Include(p => p.WorkingHours)
                .Where(p =>
                    p.Latitude >= -90 && p.Latitude <= 90 &&
                    p.Longitude >= -180 && p.Longitude <= 180 &&
                    !(p.Latitude == 0 && p.Longitude == 0))
                .AsQueryable();

            //  Temalar (varsa)
            if (prefs != null && !string.IsNullOrWhiteSpace(prefs.PreferredThemes))
            {
                var mappedCategories = prefs.PreferredThemes
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(t => _themeMap.ContainsKey(t))
                    .Select(t => _themeMap[t])
                    .Distinct()
                    .ToList();

                if (mappedCategories.Any())
                    poisQuery = poisQuery.Where(p => mappedCategories.Contains(p.Category));
            }

            //  Min rating (varsa) — review’suzları dışlama YAPMIYORUZ (kapsayıcı)
            if (prefs?.MinPoiRating > 0)
            {
                var min = prefs.MinPoiRating;
                poisQuery = poisQuery.Where(p =>
                    !p.Reviews.Any() || p.Reviews.Average(r => r.Rating) >= min);
            }

            //  Çalışma saatleri — saat bilgisi yoksa dahil; varsa “şimdi açık” arar
            var now = DateTime.Now;
            var nowTime = TimeOnly.FromDateTime(now);
            var today = (int)now.DayOfWeek;

            poisQuery = poisQuery.Where(p =>
                !p.WorkingHours.Any() ||
                p.WorkingHours.Any(h =>
                    h.DayOfWeek == today &&
                    (h.OpenTime == null || h.OpenTime <= nowTime) &&
                    (h.CloseTime == null || h.CloseTime >= nowTime)));

            //  EF'den tipli projeksiyon (SQL’e çevrilebilir)
            var baseList = await poisQuery
                .Select(p => new PoiFlat(
                    p.Id,
                    p.Name,
                    p.Latitude,
                    p.Longitude,
                    p.Reviews.Select(r => (double?)r.Rating).Average() ?? 0.0,  // boşsa 0
                    p.Reviews.Count()
                ))
                .ToListAsync(ct);

            //   Yarıçap skorlama (client-side)
            var (chosen, usedRadius) = RankAndPickWithinRadius(baseList, userLat, userLon, radiusKm);

            if (!chosen.Any())
            {
                foreach (var fallback in new[] { 50.0, 75.0, 100.0 })
                {
                    (chosen, usedRadius) = RankAndPickWithinRadius(baseList, userLat, userLon, fallback);
                    if (chosen.Any()) break;
                }
            }

            return new
            {
                poiIds = chosen.Select(x => x.Id).ToList(),
                totalFound = chosen.Count,
                radiusKmUsed = usedRadius,
                appliedFilters = new
                {
                    hasUserPreferences = prefs != null,
                    preferredThemes = prefs?.PreferredThemes,
                    minRating = prefs?.MinPoiRating,
                    openNowAssumeIfMissing = true,
                    coordValidated = true
                }
            };
        }                

        private static (List<ScoredPoi> chosen, double usedRadiusKm)
            RankAndPickWithinRadius(
                List<PoiFlat> baseList,
                double userLat, double userLon,
                double radiusKm)
        {
            var within = baseList
                .Select(p => new ScoredPoi(
                    p.Id,
                    DistanceKm: HaversineKm(userLat, userLon, p.Latitude, p.Longitude),
                    Rating: p.AvgRating,
                    Reviews: p.ReviewCount
                ))
                .Where(x => x.DistanceKm <= radiusKm)
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Reviews)
                .ThenBy(x => x.DistanceKm)
                .Take(10)
                .ToList();

            return (within, radiusKm);
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // km
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) *
                Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
                
        private readonly record struct PoiFlat(
            int Id,
            string Name,
            double Latitude,
            double Longitude,
            double AvgRating,
            int ReviewCount);

        private readonly record struct ScoredPoi(
            int Id,
            double DistanceKm,
            double Rating,
            int Reviews);
    }
}
