import React, { useState, useEffect } from 'react';
import { config, updateDevConfig } from '../../config/environment';

const PortConfig = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [backendPort, setBackendPort] = useState(config.backendPort);
  const [frontendPort, setFrontendPort] = useState(config.frontendPort);
  const [useHttps, setUseHttps] = useState(true);

  useEffect(() => {
    // Mevcut yapılandırmayı yükle
    const savedConfig = localStorage.getItem('devConfig');
    if (savedConfig) {
      try {
        const parsed = JSON.parse(savedConfig);
        setBackendPort(parsed.backendPort || config.backendPort);
        setFrontendPort(parsed.frontendPort || config.frontendPort);
        setUseHttps(parsed.apiUrl?.includes('https') ?? true);
      } catch (e) {
        console.warn('Config parse error:', e);
      }
    }
  }, []);

  const handleSave = () => {
    const protocol = useHttps ? 'https' : 'http';
    const newConfig = {
      apiUrl: `${protocol}://localhost:${backendPort}/api`,
      backendPort,
      frontendPort
    };
    
    updateDevConfig(newConfig);
  };

  const handleReset = () => {
    localStorage.removeItem('devConfig');
    window.location.reload();
  };

  if (process.env.NODE_ENV !== 'development') {
    return null; // Sadece development ortamında göster
  }

  return (
    <div className="fixed bottom-4 right-4 z-50">
      {/* Toggle Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="bg-blue-600 hover:bg-blue-700 text-white rounded-full p-3 shadow-lg"
        title="Port Yapılandırması"
      >
        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
        </svg>
      </button>

      {/* Config Panel */}
      {isOpen && (
        <div className="absolute bottom-16 right-0 bg-white rounded-lg shadow-xl p-6 w-80 border">
          <h3 className="text-lg font-semibold mb-4 text-gray-800">
            🔧 Port Yapılandırması
          </h3>
          
          <div className="space-y-4">
            {/* Backend Port */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Backend Port
              </label>
              <input
                type="number"
                value={backendPort}
                onChange={(e) => setBackendPort(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="7248"
              />
            </div>

            {/* Frontend Port */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Frontend Port
              </label>
              <input
                type="number"
                value={frontendPort}
                onChange={(e) => setFrontendPort(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="3000"
              />
            </div>

            {/* HTTPS Toggle */}
            <div className="flex items-center">
              <input
                type="checkbox"
                id="useHttps"
                checked={useHttps}
                onChange={(e) => setUseHttps(e.target.checked)}
                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <label htmlFor="useHttps" className="ml-2 block text-sm text-gray-700">
                HTTPS Kullan
              </label>
            </div>

            {/* Current API URL */}
            <div className="bg-gray-50 p-3 rounded-md">
              <p className="text-xs text-gray-600 mb-1">Mevcut API URL:</p>
              <p className="text-sm font-mono text-gray-800 break-all">
                {useHttps ? 'https' : 'http'}://localhost:{backendPort}/api
              </p>
            </div>

            {/* Action Buttons */}
            <div className="flex space-x-2 pt-2">
              <button
                onClick={handleSave}
                className="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded-md text-sm font-medium transition-colors"
              >
                Kaydet
              </button>
              <button
                onClick={handleReset}
                className="flex-1 bg-gray-500 hover:bg-gray-600 text-white py-2 px-4 rounded-md text-sm font-medium transition-colors"
              >
                Sıfırla
              </button>
            </div>
          </div>

          {/* Info */}
          <div className="mt-4 p-3 bg-blue-50 rounded-md">
            <p className="text-xs text-blue-700">
              💡 Bu ayarlar sadece sizin tarayıcınızda saklanır ve diğer geliştiricileri etkilemez.
            </p>
          </div>
        </div>
      )}
    </div>
  );
};

export default PortConfig;
