using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly UserPreferencesService _service;

        public UserPreferencesController(UserPreferencesService service)
        {
            _service = service;
        }

        // Kullanıcıya ait tercihleri getir
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserPreferences>> Get(int userId)
        {
            var prefs = await _service.GetByUserIdAsync(userId);
            if (prefs == null) return NotFound();
            return Ok(prefs);
        }

        // Kullanıcı tercihlerini kaydet/güncelle
        [HttpPost]
        public async Task<ActionResult<UserPreferences>> Save(UserPreferencesDto dto)
        {
            var prefs = new UserPreferences
            {
                CrowdednessPreference = dto.CrowdednessPreference,
                MaxWalkDistance = dto.MaxWalkDistance,
                PreferredThemes = string.Join(",", dto.PreferredThemes),
                PreferredTransportationMode = dto.PreferredTransportationMode,
                MinStartTimeHour = dto.MinStartTimeHour,
                MaxEndTimeHour = dto.MaxEndTimeHour,
                MinPoiRating = dto.MinPoiRating,
                ConsiderTraffic = dto.ConsiderTraffic,
                PrioritizeShortestRoute = dto.PrioritizeShortestRoute,
                AccessibilityFriendly = dto.AccessibilityFriendly
            };

            var saved = await _service.SaveAsync(prefs);
            return Ok(saved);
        }
    }
}
