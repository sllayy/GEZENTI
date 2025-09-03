
// src/setupProxy.js
const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'https://localhost:5136', // ASP.NET Kestrel portu
      changeOrigin: true,
      secure: false,      // self-signed sertifika için
      logLevel: 'debug',
    })
  );
};