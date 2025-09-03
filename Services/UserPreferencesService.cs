#nullable enable
using GeziRotasi.API.Data;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GeziRotasi.API.Services
{
    public class UserPreferencesService
    {
        private readonly AppDbContext _db;

        public UserPreferencesService(AppDbContext db)
        {
            _db = db;
        }

        // Belirli bir kullanıcının tercihlerini getir
        public async Task<UserPreferences?> GetByUserIdAsync(int userId)
        {
            return await _db.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // Kullanıcı tercihlerini kaydet/güncelle
        public async Task<UserPreferences> SaveAsync(UserPreferences prefs)
        {
            var existing = await _db.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == prefs.UserId);

            if (existing == null)
            {
                // Yeni kayıt ekle
                _db.UserPreferences.Add(prefs);
            }
            else
            {
                // Sadece güncellenebilir alanları güncelle
                existing.CrowdednessPreference = prefs.CrowdednessPreference;
                existing.MaxWalkDistance = prefs.MaxWalkDistance;
                existing.PreferredThemes = prefs.PreferredThemes;
                existing.PreferredTransportationMode = prefs.PreferredTransportationMode;
                existing.MinStartTimeHour = prefs.MinStartTimeHour;
                existing.MaxEndTimeHour = prefs.MaxEndTimeHour;
                existing.MinPoiRating = prefs.MinPoiRating;
                existing.ConsiderTraffic = prefs.ConsiderTraffic;
                existing.PrioritizeShortestRoute = prefs.PrioritizeShortestRoute;
                existing.AccessibilityFriendly = prefs.AccessibilityFriendly;
            }

            await _db.SaveChangesAsync();
            return existing ?? prefs;
        }
    }
}
