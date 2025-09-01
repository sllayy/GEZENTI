import axios from 'axios';
import { getApiUrl } from '../config/environment';

const api = axios.create({
  baseURL: getApiUrl(),
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: false, // cookie gönderilmesin
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('jwtToken'); // localStorage'daki token
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Auth API functions
export const authAPI = {
  // Register function
  register: async (userData) => {
    const response = await api.post('/Auth/register', userData);
    return response.data;
  },

  // Login function
  login: async (credentials) => {
    const response = await api.post('/Auth/login', credentials);
    return response.data;
  },

  // Confirm email function
  confirmEmail: async (emailData) => {
    const response = await api.post('/Auth/confirm-email', emailData);
    return response.data;
  },

  // Resend confirmation code
  resendCode: async (resendData) => {
    const response = await api.post('/Auth/resend-code', resendData);
    return response.data;
  },

  // Get current user info
  getMe: async () => {
    const response = await api.get('/Auth/me');
    return response.data;
  }
};

export default api;
