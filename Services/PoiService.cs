#nullable enable
using GeziRotasi.API.Data;
using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Point = NetTopologySuite.Geometries.Point;
using FeatureCollection = NetTopologySuite.Features.FeatureCollection;

namespace GeziRotasi.API.Services
{
    public class PoiService
    {
        private readonly AppDbContext _context;

        public PoiService(AppDbContext context)
        {
            _context = context;
        }

        // --- OKUMA (READ) İŞLEMLERİ ---
        public async Task<List<Poi>> GetAllPoisAsync(
            string? category,
            string? searchTerm,
            string? sortBy,
            string? sortDirection,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Pois.AsQueryable();

            // Kategoriye göre filtreleme (string → enum parse)
            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<PoiCategory>(category, true, out var categoryEnum))
            {
                query = query.Where(p => p.Category == categoryEnum);
            }

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(searchTerm.ToLower())) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm.ToLower()))
                );
            }

            // Sıralama
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool isDescending = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
                if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                }
                else if (sortBy.Equals("category", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDescending ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category);
                }
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            var itemsToSkip = (pageNumber - 1) * pageSize;
            return await query.Skip(itemsToSkip).Take(pageSize).ToListAsync();
        }

        public async Task<Poi?> GetPoiByIdAsync(int id)
        {
            return await _context.Pois
                .Include(p => p.WorkingHours)
                .Include(p => p.SpecialDayHours)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // --- YAZMA (CREATE, UPDATE, DELETE) İŞLEMLERİ ---
        public async Task<Poi> CreatePoiAsync(Poi newPoi)
        {
            _context.Pois.Add(newPoi);
            await _context.SaveChangesAsync();
            return newPoi;
        }

        public async Task UpdatePoiAsync(int id, Poi updatedPoi)
        {
            var poiToUpdate = await _context.Pois.FindAsync(id);
            if (poiToUpdate != null)
            {
                poiToUpdate.Name = updatedPoi.Name;
                poiToUpdate.Description = updatedPoi.Description;
                poiToUpdate.Latitude = updatedPoi.Latitude;
                poiToUpdate.Longitude = updatedPoi.Longitude;
                poiToUpdate.Category = updatedPoi.Category; // ✅ enum
                poiToUpdate.OpeningHours = updatedPoi.OpeningHours;
                poiToUpdate.Website = updatedPoi.Website;
                poiToUpdate.PhoneNumber = updatedPoi.PhoneNumber;
                poiToUpdate.GirisUcreti = updatedPoi.GirisUcreti;
                poiToUpdate.OrtalamaZiyaretSuresi = updatedPoi.OrtalamaZiyaretSuresi;
                poiToUpdate.ImageUrl = updatedPoi.ImageUrl;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePoiAsync(int id)
        {
            var poiToDelete = await _context.Pois.FindAsync(id);
            if (poiToDelete != null)
            {
                _context.Pois.Remove(poiToDelete);
                await _context.SaveChangesAsync();
            }
        }

        // --- OSM ENTEGRASYON İŞLEMLERİ ---
        public async Task<bool> IsOsmIdExistsAsync(long osmId)
        {
            return await _context.Pois.AnyAsync(p => p.ExternalApiId == osmId.ToString());
        }

        public async Task<Poi> ImportFromOsmAsync(OsmElement osmElement)
        {
            var amenity = osmElement.Tags.GetValueOrDefault("amenity", "Genel");
            Enum.TryParse<PoiCategory>(amenity, true, out var poiCategory);

            var newPoi = new Poi
            {
                Name = osmElement.Tags.GetValueOrDefault("name", "İsimsiz Mekan"),
                Description = "OpenStreetMap üzerinden eklendi.",
                Latitude = osmElement.Lat,
                Longitude = osmElement.Lon,
                Category = poiCategory, // ✅ enum
                ExternalApiId = osmElement.Id.ToString(),
                OpeningHours = osmElement.Tags.GetValueOrDefault("opening_hours"),
                Website = osmElement.Tags.GetValueOrDefault("website"),
                PhoneNumber = osmElement.Tags.GetValueOrDefault("phone")
            };
            return await CreatePoiAsync(newPoi);
        }

        // --- ÇALIŞMA SAATLERİ İŞLEMLERİ ---
        public async Task<List<WorkingHour>> GetWorkingHoursForPoiAsync(int poiId)
        {
            return await _context.WorkingHours
                .Where(wh => wh.PoiId == poiId)
                .ToListAsync();
        }

        public async Task<WorkingHour> AddWorkingHourAsync(WorkingHour workingHour)
        {
            _context.WorkingHours.Add(workingHour);
            await _context.SaveChangesAsync();
            return workingHour;
        }

        public async Task<List<SpecialDayHour>> GetSpecialDayHoursForPoiAsync(int poiId)
        {
            return await _context.SpecialDayHours
                .Where(sdh => sdh.PoiId == poiId)
                .OrderBy(sdh => sdh.Date)
                .ToListAsync();
        }

        public async Task<SpecialDayHour> AddSpecialDayHourAsync(SpecialDayHour specialDayHour)
        {
            _context.SpecialDayHours.Add(specialDayHour);
            await _context.SaveChangesAsync();
            return specialDayHour;
        }

        // --- GEOJSON DÖNÜŞTÜRME ---
        public string ConvertPoisToGeoJson(List<Poi> pois)
        {
            var features = new List<Feature>();
            foreach (var poi in pois)
            {
                var point = new Point(poi.Longitude, poi.Latitude);
                var attributes = new AttributesTable
                {
                    { "id", poi.Id },
                    { "name", poi.Name },
                    { "description", poi.Description },
                    { "category", poi.Category.ToString() }, // ✅ enum → string
                    { "imageUrl", poi.ImageUrl }
                };
                features.Add(new Feature(point, attributes));
            }
            var featureCollection = new FeatureCollection();
            features.ForEach(f => featureCollection.Add(f));
            return new GeoJsonWriter().Write(featureCollection);
        }
    }
}
