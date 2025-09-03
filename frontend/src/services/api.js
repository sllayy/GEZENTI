// src/services/api.js
import axios from "axios";
import { getApiUrl } from "../config/environment";

const api = axios.create({
  baseURL: getApiUrl(),
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000,
  withCredentials: false,
});

// 🔑 JWT Interceptor
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("jwtToken"); // login sonrası buraya kaydedilmeli
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
  register: (userData) => api.post("/Auth/register", userData).then((res) => res.data),
  login: async (credentials) => {
    const res = await api.post("/Auth/login", credentials);
    if (res.data?.token) {
      localStorage.setItem("jwtToken", res.data.token); // ✅ login sonrası token kaydet
    }
    return res.data;
  },
  confirmEmail: (emailData) => api.post("/Auth/confirm-email", emailData).then((r) => r.data),
  resendCode: (resendData) => api.post("/Auth/resend-code", resendData).then((r) => r.data),
  getMe: () => api.get("/Auth/me").then((r) => r.data),
  updateProfile: (data) => api.post("/Auth/update-profile", data).then((r) => r.data),
  changePassword: (data) => api.post("/Auth/change-password", data).then((r) => r.data),
  deleteAccount: (data) => api.post("/Auth/delete-account", data).then((r) => r.data),
  forgotPassword: (data) => api.post("/Auth/forgot-password", data).then((r) => r.data),
  resetPassword: (data) => api.post("/Auth/reset-password", data).then((r) => r.data),
  unlockAccount: (data) => api.post("/Auth/unlock-account", data).then((r) => r.data),
  googleFirebase: (idToken) => api.post("/Auth/google-firebase", { idToken }).then((r) => r.data),
};

// -------------------------
// USER PREFERENCES API
// -------------------------
export const userPreferencesAPI = {
  get: (userId) => api.get(`/UserPreferences/${userId}`).then((r) => r.data),
  save: (prefs) => api.post("/UserPreferences", prefs).then((r) => r.data),
};

// -------------------------
// ROUTE API
// -------------------------
export const routeAPI = {
  optimize: (payload) => api.post("/route/optimize", payload).then((r) => r.data),
  getRoute: (payload) => api.post("/MapRoute", payload).then((r) => r.data),
  getRouteWithPois: (payload) => api.post("/MapRoute/with-pois", payload).then((r) => r.data),
  save: (payload) => api.post("/MapRoute/save", payload).then((r) => r.data),
};

// -------------------------
// POI API
// -------------------------
export const poisAPI = {
  searchNearby: (lat, lon, type, radius = 1000) =>
    api.get("/Pois/search-nearby", { params: { lat, lon, type, radius } }).then((r) => r.data),
  getById: (id) => api.get(`/Pois/${id}`).then((r) => r.data),
};

export default api;
