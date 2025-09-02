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
                _db.UserPreferences.Add(prefs);
            }
            else
            {
                // Budget alanı kaldırıldığı için sadece diğer alanları güncelle
                _db.Entry(existing).CurrentValues.SetValues(prefs);
            }

            await _db.SaveChangesAsync();
            return prefs;
        }
    }
}
