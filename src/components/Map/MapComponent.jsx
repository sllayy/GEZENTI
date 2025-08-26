import React, { useRef, useState, useEffect } from 'react';
import mapboxgl from 'mapbox-gl';
import 'mapbox-gl/dist/mapbox-gl.css'; // Mapbox stillerinin doğru çalışması için bu satır önemlidir.

import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

// TypeScript'e özgü interface tanımı JSX'te kullanılmaz.
// props'lar doğrudan fonksiyon parametresi olarak alınır.
const MapComponent = ({ mapboxToken, onTokenSubmit }) => {
  // useRef'ten generic tip (<...>) kaldırıldı.
  const mapContainer = useRef(null);
  const map = useRef(null);
  const [tokenInput, setTokenInput] = useState('');
  const [userInteracting, setUserInteracting] = useState(false);

  useEffect(() => {
    if (!mapContainer.current || !mapboxToken) return;

    // Haritayı başlat
    mapboxgl.accessToken = mapboxToken;
    
    map.current = new mapboxgl.Map({
      container: mapContainer.current,
      style: 'mapbox://styles/mapbox/outdoors-v12',
      projection: 'globe', // 'as any' tip zorlaması kaldırıldı.
      zoom: 2,
      center: [35, 39], // Türkiye merkezi
      pitch: 0,
    });

    // Navigasyon kontrollerini ekle
    map.current.addControl(
      new mapboxgl.NavigationControl({
        visualizePitch: true,
      }),
      'top-right'
    );

    // Konum bulma kontrolünü ekle
    map.current.addControl(
      new mapboxgl.GeolocateControl({
        positionOptions: {
          enableHighAccuracy: true
        },
        trackUserLocation: true,
        showUserHeading: true
      }),
      'top-right'
    );

    // Atmosfer ve sis efektlerini ekle
    map.current.on('style.load', () => {
      map.current?.setFog({
        color: 'rgb(220, 220, 255)',
        'high-color': 'rgb(200, 200, 235)',
        'horizon-blend': 0.15,
      });
    });

    // Örnek POI (İlgi Çekici Nokta) işaretçileri
    const samplePOIs = [
      { coordinates: [28.9784, 41.0082], name: 'İstanbul', description: 'Tarihi şehir' },
      { coordinates: [32.8597, 39.9334], name: 'Ankara', description: 'Başkent' },
      { coordinates: [27.1428, 38.4237], name: 'İzmir', description: 'Ege\'nin incisi' },
      { coordinates: [30.7133, 36.8969], name: 'Antalya', description: 'Turizm merkezi' }
    ];

    samplePOIs.forEach(poi => {
      const marker = new mapboxgl.Marker({
        color: '#1d98d1'
      })
        // 'as [number, number]' tip zorlaması kaldırıldı.
        .setLngLat(poi.coordinates)
        .setPopup(
          new mapboxgl.Popup({ offset: 25 })
            .setHTML(`<h3 class="font-bold">${poi.name}</h3><p>${poi.description}</p>`)
        )
        // '!' (non-null assertion) kaldırıldı, çünkü map.current bu scope'ta null olamaz.
        .addTo(map.current);
    });

    // Etkileşim için olay dinleyicileri
    map.current.on('mousedown', () => setUserInteracting(true));
    map.current.on('dragstart', () => setUserInteracting(true));
    map.current.on('mouseup', () => setUserInteracting(false));
    map.current.on('touchend', () => setUserInteracting(false));

    // Component kaldırıldığında haritayı temizle
    return () => {
      map.current?.remove();
    };
  }, [mapboxToken]);

  const handleTokenSubmit = () => {
    if (tokenInput.trim()) {
      onTokenSubmit?.(tokenInput.trim());
    }
  };

  // Geriye kalan JSX yapısı tamamen aynıdır.
  if (!mapboxToken) {
    return (
      <div className="flex items-center justify-center min-h-[500px] bg-muted/30 rounded-lg">
        <Card className="w-full max-w-md">
          <CardContent className="p-6">
            <div className="text-center mb-4">
              <i className="pi pi-map text-4xl text-primary mb-3 block"></i>
              <h3 className="text-lg font-semibold mb-2">Mapbox Token Gerekli</h3>
              <p className="text-sm text-muted-foreground mb-4">
                Harita görüntülemek için Mapbox public token'ınızı girin.
                <br />
                <a 
                  href="https://mapbox.com/" 
                  target="_blank" 
                  rel="noopener noreferrer"
                  className="text-primary hover:underline"
                >
                  mapbox.com
                </a> adresinden token alabilirsiniz.
              </p>
            </div>
            <div className="space-y-3">
              <Input
                placeholder="pk.eyJ1Ijo..."
                value={tokenInput}
                onChange={(e) => setTokenInput(e.target.value)}
                type="password"
              />
              <Button 
                onClick={handleTokenSubmit}
                className="w-full bg-gradient-hero hover:shadow-adventure"
                disabled={!tokenInput.trim()}
              >
                Haritayı Yükle
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="relative w-full h-[500px] lg:h-[600px] rounded-lg overflow-hidden shadow-adventure">
      <div ref={mapContainer} className="absolute inset-0" />
      
      {/* Harita Kontrolleri */}
      <div className="absolute top-4 left-4 z-10">
        <Card className="bg-card/90 backdrop-blur-sm">
          <CardContent className="p-3">
            <div className="flex items-center space-x-2 text-sm">
              <i className="pi pi-info-circle text-primary"></i>
              <span>Harita üzerinde POI'lara tıklayın</span>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Rota Planlama Araçları */}
      <div className="absolute bottom-4 left-4 right-4 z-10">
        <Card className="bg-card/90 backdrop-blur-sm">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <Button size="sm" className="bg-gradient-nature">
                  <i className="pi pi-plus mr-2"></i>
                  Rota Başlat
                </Button>
                <Button size="sm" variant="outline">
                  <i className="pi pi-search mr-2"></i>
                  POI Ara
                </Button>
              </div>
              <div className="text-sm text-muted-foreground">
                <i className="pi pi-map-marker mr-1"></i>
                4 POI görüntüleniyor
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default MapComponent;