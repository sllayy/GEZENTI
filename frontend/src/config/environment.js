// Environment Configuration
// Bu dosya her geliştirici için farklı port yapılandırması sağlar

const getEnvironmentConfig = () => {
  // Development ortamında environment variables'ları kontrol et
  const apiUrl = process.env.REACT_APP_API_URL;
  const backendPort = process.env.REACT_APP_BACKEND_PORT;
  const frontendPort = process.env.REACT_APP_FRONTEND_PORT;

  // Eğer environment variable yoksa, varsayılan değerleri kullan
  if (apiUrl) {
    return {
      apiUrl,
      backendPort: backendPort || '7248',
      frontendPort: frontendPort || '3000'
    };
  }

  // Varsayılan yapılandırma (herkes kendi portunu burada değiştirebilir)
  const defaultConfig = {
    apiUrl: 'http://localhost:7248/api',
    backendPort: '7248',
    frontendPort: '3000'
  };

  // Local storage'dan özel yapılandırma kontrol et
  const savedConfig = localStorage.getItem('devConfig');
  if (savedConfig) {
    try {
      const parsed = JSON.parse(savedConfig);
      return { ...defaultConfig, ...parsed };
    } catch (e) {
      console.warn('Saved config parse error:', e);
    }
  }

  return defaultConfig;
};

// Geliştirici için port yapılandırmasını değiştirme fonksiyonu
export const updateDevConfig = (config) => {
  localStorage.setItem('devConfig', JSON.stringify(config));
  window.location.reload(); // Sayfayı yenile
};

// Mevcut yapılandırmayı al
export const config = getEnvironmentConfig();

// API URL'ini döndür
export const getApiUrl = () => config.apiUrl;

// Backend port'unu döndür
export const getBackendPort = () => config.backendPort;

// Frontend port'unu döndür
export const getFrontendPort = () => config.frontendPort;

export default config;
