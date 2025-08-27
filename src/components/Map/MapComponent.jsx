// src/components/Map/MapComponent.jsx
import React, { useEffect, useRef, useState } from 'react';
import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import { fromLonLat } from 'ol/proj';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import { Style, Icon, Stroke } from 'ol/style';
import 'ol/ol.css';
import LocationInput from './LocationInput';
import { FaRoute, FaPlus } from 'react-icons/fa';

// POI noktaları
const pois = [
  { id: 1, name: 'Kapadokya', location: [34.8569, 38.6493] },
  { id: 2, name: 'Pamukkale', location: [29.1245, 37.9298] },
  { id: 3, name: 'Efes', location: [27.3411, 37.9408] },
  { id: 4, name: 'Boğaziçi', location: [29.0494, 41.0888] },
];

const MapComponent = () => {
  const mapElement = useRef(null);
  const [map, setMap] = useState(null);

  // Başlangıç ve bitiş noktalarını tutan state'ler
  const [startPoint, setStartPoint] = useState(null);
  const [endPoint, setEndPoint] = useState(null);

  // Harita üzerinde işaretçileri yönetmek için vektör kaynağı
  const markerSource = new VectorSource();
  const markerLayer = new VectorLayer({
    source: markerSource,
  });

  useEffect(() => {
    if (mapElement.current) {
      const initialMap = new Map({
        target: mapElement.current,
        layers: [
          new TileLayer({
            source: new OSM(),
          }),
          markerLayer,
        ],
        view: new View({
          center: fromLonLat([35.2433, 38.9637]),
          zoom: 6,
        }),
      });
      setMap(initialMap);

      return () => initialMap.setTarget(undefined);
    }
  }, []);

  // Başlangıç/bitiş noktaları değiştiğinde haritayı ve markerları güncelleme
  useEffect(() => {
    if (map) {
      markerSource.clear(); // Önceki işaretçileri temizle

      if (startPoint) {
        const startFeature = new Feature({
          geometry: new Point(fromLonLat(startPoint.coords)),
        });
        startFeature.setStyle(new Style({
          image: new Icon({
            src: 'https://cdn.rawgit.com/openlayers/openlayers.github.io/master/en/v5.3.0/examples/data/icon.png',
            color: '#FF0000',
            scale: 0.8
          }),
        }));
        markerSource.addFeature(startFeature);
        map.getView().animate({ center: fromLonLat(startPoint.coords), zoom: 10 });
      }

      if (endPoint) {
        const endFeature = new Feature({
          geometry: new Point(fromLonLat(endPoint.coords)),
        });
        endFeature.setStyle(new Style({
          image: new Icon({
            src: 'https://cdn.rawgit.com/openlayers/openlayers.github.io/master/en/v5.3.0/examples/data/icon.png',
            color: '#008000',
            scale: 0.8
          }),
        }));
        markerSource.addFeature(endFeature);
      }
    }
  }, [map, startPoint, endPoint, markerSource]);

  return (
    <div className="flex flex-col min-h-screen bg-gray-100">
      {/* Başlık ve Açıklama */}
      <div className="text-center px-6 md:px-16 pt-8">
        <h2 className="text-4xl font-extrabold text-gray-800">
          İnteraktif Harita
        </h2>
        <p className="text-lg text-gray-600 mt-2">
          POI noktalarını keşfedin ve rotalarınızı planlayın.
        </p>
      </div>

      {/* Harita kutusu */}
      <div className="px-6 md:px-16 mt-6">
        <div
          ref={mapElement}
          className="w-full h-[70vh] bg-white rounded-xl shadow-lg overflow-hidden"
        />
      </div>

      {/* Ana İçerik Bölümü: Rota Planlama ve Yakındaki POI'lar */}
      <div className="flex justify-center flex-1 px-8 py-8 md:px-16">
        <div className="flex flex-col md:flex-row gap-8 w-full max-w-4xl">
          {/* Sol Kolon: Rota Planlama */}
          <div className="flex-1 bg-white p-6 rounded-xl shadow-md border">
            <h3 className="text-2xl font-semibold text-gray-700 mb-6 flex items-center gap-3">
              <FaRoute className="text-blue-500 text-3xl" />
              Rota Planlama
            </h3>
            <div className="flex flex-col gap-5">
              <LocationInput
                placeholder="Başlangıç noktanızı seçin..."
                value={startPoint?.name}
                onSelect={setStartPoint}
                type="start"
              />
              <LocationInput
                placeholder="Bitiş noktasını seçin..."
                value={endPoint?.name}
                onSelect={setEndPoint}
                type="end"
              />
            </div>
            <button className="w-full py-3 rounded-lg text-white font-semibold bg-gradient-to-r from-orange-500 to-red-500 hover:from-orange-600 hover:to-red-600 transition mt-6">
              + Yeni Rota Oluştur
            </button>
          </div>

          {/* Sağ Kolon: Yakındaki POI'lar */}
          <div className="flex-1 bg-white p-6 rounded-xl shadow-md border">
            <h3 className="text-2xl font-semibold mb-4">Yakındaki POI'lar</h3>
            <ul className="space-y-3">
              {pois.map(poi => (
                <li
                  key={poi.id}
                  className="flex justify-between items-center text-gray-700 border-b pb-2"
                >
                  <div className="flex items-center gap-2">
                    <span className="text-blue-500">
                      <svg
                        className="w-5 h-5"
                        fill="currentColor"
                        viewBox="0 0 20 20"
                      >
                        <path d="M10 2a6 6 0 00-6 6c0 4.47 6 12 6 12s6-7.53 6-12a6 6 0 00-6-6zm0 8a2 2 0 110-4 2 2 0 010 4z" />
                      </svg>
                    </span>
                    {poi.name}
                  </div>
                  <button className="text-gray-500 hover:text-blue-600">
                    <FaPlus />
                  </button>
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );


};

export default MapComponent;
