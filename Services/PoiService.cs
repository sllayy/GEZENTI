using GeziRotasi.API.Dtos;
using GeziRotasi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeziRotasi.API.Services
{
    public class PoiService
    {
        private static readonly List<Poi> _pois = new List<Poi>
        {
            new Poi { Id = 1, Name = "Topkapı Sarayı", Description = "...", Latitude = 41.0116, Longitude = 28.9834, Category = "Müze" },
            new Poi { Id = 2, Name = "Kapalıçarşı", Description = "...", Latitude = 41.0107, Longitude = 28.9681, Category = "Alışveriş" },
            new Poi { Id = 3, Name = "Sultanahmet Camii", Description = "...", Latitude = 41.0054, Longitude = 28.9768, Category = "Tarihi Yapı" },
            new Poi { Id = 4, Name = "Ayasofya Tarih ve Deneyim Müzesi", Description = "...", Latitude = 41.0086, Longitude = 28.9800, Category = "Müze" },
            new Poi { Id = 5, Name = "Yerebatan Sarnıcı", Description = "...", Latitude = 41.0084, Longitude = 28.9779, Category = "Müze" },
            new Poi { Id = 6, Name = "Galata Kulesi", Description = "...", Latitude = 41.0256, Longitude = 28.9744, Category = "Tarihi Yapı" },
            new Poi { Id = 7, Name = "Mısır Çarşısı", Description = "...", Latitude = 41.0163, Longitude = 28.9706, Category = "Alışveriş" },
            new Poi { Id = 8, Name = "Dolmabahçe Sarayı", Description = "...", Latitude = 41.0392, Longitude = 29.0001, Category = "Müze" },
            new Poi { Id = 9, Name = "Hafız Mustafa 1864", Description = "...", Latitude = 41.0101, Longitude = 28.9757, Category = "Restoran" },
            new Poi { Id = 10, Name = "Gülhane Parkı", Description = "...", Latitude = 41.0135, Longitude = 28.9822, Category = "Park" },
            new Poi { Id = 11, Name = "Türk ve İslam Eserleri Müzesi", Description = "...", Latitude = 41.0062, Longitude = 28.9750, Category = "Müze" },
            new Poi { Id = 12, Name = "Seven Hills Restaurant", Description = "...", Latitude = 41.0067, Longitude = 28.9785, Category = "Restoran" }
        };

        public List<Poi> GetAllPois(string? category, string? searchTerm, string? sortBy, string? sortDirection, int pageNumber = 1, int pageSize = 10)
        {
            var query = _pois.AsQueryable();
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => (p.Name != null && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) || (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool isDescending = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
                if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }
            var itemsToSkip = (pageNumber - 1) * pageSize;
            query = query.Skip(itemsToSkip).Take(pageSize);
            return query.ToList();
        }
        public Poi? GetPoiById(int id) => _pois.FirstOrDefault(p => p.Id == id);
        public Poi CreatePoi(Poi newPoi) { newPoi.Id = _pois.Any() ? _pois.Max(p => p.Id) + 1 : 1; _pois.Add(newPoi); return newPoi; }
        public void UpdatePoi(Poi updatedPoi) { var index = _pois.FindIndex(p => p.Id == updatedPoi.Id); if (index != -1) { _pois[index] = updatedPoi; } }
        public void DeletePoi(int id) { var poiToDelete = _pois.FirstOrDefault(p => p.Id == id); if (poiToDelete != null) { _pois.Remove(poiToDelete); } }
        public bool IsOsmIdExists(long osmId) => _pois.Any(p => p.ExternalApiId == osmId.ToString());
        public Poi ImportFromOsm(OsmElement osmElement)
        {
            var newPoi = new Poi { Name = osmElement.Tags.GetValueOrDefault("name", "İsimsiz Mekan"), Description = "OpenStreetMap üzerinden eklendi.", Latitude = osmElement.Lat, Longitude = osmElement.Lon, Category = osmElement.Tags.GetValueOrDefault("amenity", "Genel"), ExternalApiId = osmElement.Id.ToString(), OpeningHours = osmElement.Tags.GetValueOrDefault("opening_hours"), Website = osmElement.Tags.GetValueOrDefault("website"), PhoneNumber = osmElement.Tags.GetValueOrDefault("phone") };
            return CreatePoi(newPoi);
        }
    }
}