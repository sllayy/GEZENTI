// src/services/RouteFilterService.js
import axios from "axios";

// 🌍 API adresi .env dosyanda REACT_APP_API_BASE_URL şeklinde tanımlı olmalı
// Örn: REACT_APP_API_BASE_URL=http://localhost:7248
const API_URL = process.env.REACT_APP_API_BASE_URL || "http://localhost:5136";

/**
 * Filtreleme için backend'e istek gönderir.
 * @param {Object} filter - RouteFilterDto ile uyumlu nesne
 * @param {string[]} filter.categories - Seçilen kategoriler
 * @param {number|null} filter.minRating - Minimum puan
 * @param {number|null} filter.maxDistanceKm - Maksimum mesafe (km)
 * @param {string} filter.duration - Süre ("short","medium","long","multi")
 * @returns {Promise<any[]>} - Filtrelenen TravelRoute listesi
 */
export async function filterRoutes(filter) {
  try {
    const response = await axios.post(`${API_URL}/api/travelroutes/filter`, filter, {
      headers: {
        "Content-Type": "application/json",
      },
      withCredentials: true, // CORS için gerekebilir
    });
    return response.data;
  } catch (error) {
    console.error("Filter API çağrısı başarısız:", error);
    throw error;
  }
}

/**
 * Tüm travel routes verilerini getirir (filtreleme olmadan).
 * @returns {Promise<any[]>}
 */
export async function getTravelRoutes() {
  try {
    const response = await axios.get(`${API_URL}/api/travelroutes`, {
      withCredentials: true,
    });
    return response.data;
  } catch (error) {
    console.error("GetAll API çağrısı başarısız:", error);
    throw error;
  }
}

/**
 * Belirli bir id’ye göre route getirir.
 * @param {number} id
 * @returns {Promise<any>}
 */
export async function getRouteById(id) {
  try {
    const response = await axios.get(`${API_URL}/api/travelroutes/${id}`, {
      withCredentials: true,
    });
    return response.data;
  } catch (error) {
    console.error(`GetById API çağrısı başarısız (id: ${id}):`, error);
    throw error;
  }
}
