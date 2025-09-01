// src/services/api.js
import axios from 'axios';
import { getApiUrl } from '../config/environment';

const api = axios.create({
    baseURL: getApiUrl(),
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: false, // cookie gönderilmesin
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

export const authAPI = {
    // ✅ Register
    register: async (userData) => {
        const response = await api.post('/Auth/register', userData);
        return response.data;
    },

    // ✅ Login
    login: async (credentials) => {
        const response = await api.post('/Auth/login', credentials);
        return response.data;
    },

    // ✅ Confirm Email
    confirmEmail: async (emailData) => {
        const response = await api.post('/Auth/confirm-email', emailData);
        return response.data;
    },

    // ✅ Resend Code
    resendCode: async (resendData) => {
        const response = await api.post('/Auth/resend-code', resendData);
        return response.data;
    },

    // ✅ Get Me
    getMe: async () => {
        const response = await api.get('/Auth/me');
        return response.data;
    },

    // ✅ Update Profile
    updateProfile: async (data) => {
        const response = await api.post('/Auth/update-profile', data);
        return response.data;
    },

    // ✅ Change Password
    changePassword: async (data) => {
        const response = await api.post('/Auth/change-password', data);
        return response.data;
    },

    // ✅ Delete Account
    deleteAccount: async (data) => {
        const response = await api.post('/Auth/delete-account', data);
        return response.data;
    },

    // ✅ Forgot Password
    forgotPassword: async (data) => {
        const response = await api.post('/Auth/forgot-password', data);
        return response.data;
    },

    // ✅ Reset Password
    resetPassword: async (data) => {
        const response = await api.post('/Auth/reset-password', data);
        return response.data;
    },

    // ✅ Unlock Account
    unlockAccount: async (data) => {
        const response = await api.post('/Auth/unlock-account', data);
        return response.data;
    },

    // ✅ Google Firebase login/register
    googleFirebase: async (idToken) => {
        const response = await api.post('/Auth/google-firebase', { idToken });
        return response.data;
    }
};

export default api;
