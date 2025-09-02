// src/services/api.js
import axios from 'axios';
import { getApiUrl } from '../config/environment';

const api = axios.create({
    baseURL: getApiUrl(),
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
    withCredentials: false,  
});

// JWT token interceptor
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// -------------------------
// AUTH API
// -------------------------
export const authAPI = {
  register: async (userData) => (await api.post('/Auth/register', userData)).data,
  login: async (credentials) => (await api.post('/Auth/login', credentials)).data,
  confirmEmail: async (emailData) => (await api.post('/Auth/confirm-email', emailData)).data,
  resendCode: async (resendData) => (await api.post('/Auth/resend-code', resendData)).data,
  getMe: async () => (await api.get('/Auth/me')).data,
  updateProfile: async (data) => (await api.post('/Auth/update-profile', data)).data,
  changePassword: async (data) => (await api.post('/Auth/change-password', data)).data,
  deleteAccount: async (data) => (await api.post('/Auth/delete-account', data)).data,
  forgotPassword: async (data) => (await api.post('/Auth/forgot-password', data)).data,
  resetPassword: async (data) => (await api.post('/Auth/reset-password', data)).data,
  unlockAccount: async (data) => (await api.post('/Auth/unlock-account', data)).data,
  googleFirebase: async (idToken) => (await api.post('/Auth/google-firebase', { idToken })).data,
};

// -------------------------
// USER PREFERENCES API
// -------------------------
export const userPreferencesAPI = {
  get: async (userId) => (await api.get(`/UserPreferences/${userId}`)).data,
  save: async (prefs) => (await api.post('/UserPreferences', prefs)).data,
};

// -------------------------
// ROUTE API
// -------------------------
export const routeAPI = {
  optimize: async (payload) => (await api.post('/route/optimize', payload)).data,
  getRoute: async (payload) => (await api.post('/MapRoute', payload)).data,
  getRouteWithPois: async (payload) => (await api.post('/MapRoute/with-pois', payload)).data,
  save: async (payload) => (await api.post('/MapRoute/save', payload)).data,
};

// -------------------------
// POI API
// -------------------------
export const poisAPI = {
  // ✅ Yakındaki mekanları ara
  searchNearby: async (lat, lon, type, radius = 1000) => {
    const response = await api.get('/Pois/search-nearby', {
      params: { lat, lon, type, radius },
    });
    return response.data;
  },

  // ✅ POI detayı
  getById: async (id) => {
    const response = await api.get(`/Pois/${id}`);
    return response.data;
  },
};
export default api;
