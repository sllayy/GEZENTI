// src/pages/MapComponentPage.jsx
import React, { useEffect, useRef, useState } from "react";
import Map from "ol/Map";
import View from "ol/View";
import TileLayer from "ol/layer/Tile";
import OSM from "ol/source/OSM";
import VectorLayer from "ol/layer/Vector";
import VectorSource from "ol/source/Vector";
import { fromLonLat } from "ol/proj";
import Feature from "ol/Feature";
import Point from "ol/geom/Point";
import { Style, Icon, Stroke } from "ol/style";
import { LineString } from "ol/geom";
import "ol/ol.css";
import LocationInput from "../components/Map/LocationInput";
import { FaRoute, FaPlus } from "react-icons/fa";
import { routeAPI, poisAPI } from "../services/api";

const MapComponentPage = () => {
  const mapElement = useRef(null);
  const [map, setMap] = useState(null);

  const [startPoint, setStartPoint] = useState(null);
  const [endPoint, setEndPoint] = useState(null);

  const [pois, setPois] = useState([]);
  const [filteredPois, setFilteredPois] = useState([]); // ✅ filtreli liste
  const [searchTerm, setSearchTerm] = useState(""); // ✅ arama çubuğu
  const [selectedPoi, setSelectedPoi] = useState(null);
  const [routeGeometry, setRouteGeometry] = useState(null);

  const markerSource = useRef(new VectorSource()).current;
  const routeSource = useRef(new VectorSource()).current;

  const markerLayer = new VectorLayer({ source: markerSource });
  const routeLayer = new VectorLayer({
    source: routeSource,
    style: new Style({
      stroke: new Stroke({ color: "#ff6600", width: 4 }),
    }),
  });

  // Haritayı başlat
  useEffect(() => {
    if (mapElement.current) {
      const initialMap = new Map({
        target: mapElement.current,
        layers: [new TileLayer({ source: new OSM() }), routeLayer, markerLayer],
        view: new View({
          center: fromLonLat([28.97, 41.01]),
          zoom: 12,
        }),
      });
      setMap(initialMap);
      return () => initialMap.setTarget(undefined);
    }
  }, []);

  // Marker & POI çizimleri
  useEffect(() => {
    if (map) {
      markerSource.clear();

      if (startPoint) {
        const f = new Feature({
          geometry: new Point(fromLonLat(startPoint.coords)),
        });
        f.setStyle(
          new Style({
            image: new Icon({
              src: "https://cdn-icons-png.flaticon.com/512/684/684908.png",
              scale: 0.07,
            }),
          })
        );
        markerSource.addFeature(f);
      }

      if (endPoint) {
        const f = new Feature({
          geometry: new Point(fromLonLat(endPoint.coords)),
        });
        f.setStyle(
          new Style({
            image: new Icon({
              src: "https://cdn-icons-png.flaticon.com/512/149/149059.png",
              scale: 0.07,
            }),
          })
        );
        markerSource.addFeature(f);
      }

      pois.forEach((poi, index) => {
        const poiFeature = new Feature({
          geometry: new Point(fromLonLat([poi.longitude, poi.latitude])),
        });
        poiFeature.setId(poi.uniqueId || `${poi.id}-${index}`); // ✅ unique ID
        poiFeature.setStyle(
          new Style({
            image: new Icon({
              src: "https://cdn-icons-png.flaticon.com/512/149/149059.png",
              scale: 0.05,
            }),
          })
        );
        markerSource.addFeature(poiFeature);
      });
    }
  }, [map, startPoint, endPoint, pois]);

  // POI ve rota fetch
  const fetchPoisOrRoute = async (start, end = null) => {
    try {
      const body = { mode: "driving", coordinates: [start] };
      if (end) body.coordinates.push(end);

      const data = await routeAPI.getRouteWithPois(body);

      const poisFromDb = (data.visitPois || []).map((p) => ({
        ...p,
        uniqueId: `db-${p.id}`,
      }));
      setPois(poisFromDb);
      setFilteredPois(poisFromDb); // ✅ filtreyi güncelle
      setRouteGeometry(data.geometry);

      routeSource.clear();

      if (end && data.geometry?.coordinates) {
        const coords = data.geometry.coordinates.map((c) =>
          fromLonLat([c[0], c[1]])
        );
        const line = new Feature({ geometry: new LineString(coords) });
        routeSource.addFeature(line);
      }

      if (start) {
        try {
          const nearby = await poisAPI.searchNearby(
            start[1],
            start[0],
            "restaurant",
            2000
          );
          if (nearby?.elements) {
            const nearbyPois = nearby.elements.map((el, idx) => ({
              uniqueId: `osm-${el.id}-${idx}`, // ✅ index ekle
              id: el.id,
              name: el.tags?.name || "İsimsiz Mekan",
              latitude: el.lat,
              longitude: el.lon,
              category: el.tags?.amenity || "Unknown",
              avgRating: Math.random() * 5,
            }));
            setPois((prev) => [...prev, ...nearbyPois]);
            setFilteredPois((prev) => [...prev, ...nearbyPois]);
          }
        } catch (err) {
          console.warn("Yakındaki mekanlar getirilemedi:", err);
        }
      }
    } catch (err) {
      console.error("fetchPoisOrRoute hata:", err);
    }
  };

  // POI detayı getir
  const handlePoiClick = async (poi) => {
    if (poi.uniqueId.startsWith("osm-")) {
      setSelectedPoi(poi); // OSM için backend çağrısı yok
      return;
    }
    try {
      const detail = await poisAPI.getById(poi.id);
      setSelectedPoi(detail);
    } catch (err) {
      console.error("POI detayı alınamadı:", err);
      setSelectedPoi(poi);
    }
  };

  // Arama → listeyi filtrele
  useEffect(() => {
    if (!searchTerm) {
      setFilteredPois(pois);
    } else {
      setFilteredPois(
        pois.filter((p) =>
          p.name.toLowerCase().includes(searchTerm.toLowerCase())
        )
      );
    }
  }, [searchTerm, pois]);

  // İlk açılış → konum al
  useEffect(() => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (pos) => {
          const coords = [pos.coords.longitude, pos.coords.latitude];
          setStartPoint({ coords, name: "Mevcut Konum" });
          fetchPoisOrRoute(coords);
        },
        () => {
          const coords = [28.97, 41.01];
          setStartPoint({ coords, name: "İstanbul Merkez" });
          fetchPoisOrRoute(coords);
        },
        { enableHighAccuracy: true }
      );
    }
  }, []);

  const handleNewRoute = () => {
    if (!startPoint || !endPoint) {
      alert("Başlangıç ve bitiş seçin.");
      return;
    }
    fetchPoisOrRoute(startPoint.coords, endPoint.coords);
  };

  return (
    <div className="flex flex-col min-h-screen bg-gray-100">
      <div className="text-center px-6 md:px-16 pt-8">
        <h2 className="text-4xl font-extrabold text-gray-800">İnteraktif Harita</h2>
        <p className="text-lg text-gray-600 mt-2">
          Konumunu seç, yakındaki POI’leri keşfet. Rota oluşturduğunda popüler POI’ler listelenecek.
        </p>
      </div>

      <div className="px-6 md:px-16 mt-6">
        <div
          ref={mapElement}
          className="w-full h-[70vh] bg-white rounded-xl shadow-lg overflow-hidden"
        />
      </div>

      <div className="flex justify-center flex-1 px-8 py-8 md:px-16">
        <div className="flex flex-col md:flex-row gap-8 w-full max-w-5xl">
          <div className="flex-1 bg-white p-6 rounded-xl shadow-md border">
            <h3 className="text-2xl font-semibold text-gray-700 mb-6 flex items-center gap-3">
              <FaRoute className="text-blue-500 text-3xl" /> Rota Planlama
            </h3>
            <LocationInput
              placeholder="Başlangıç seç..."
              value={startPoint?.name}
              onSelect={(point) => {
                setStartPoint(point);
                if (point?.coords) {
                  fetchPoisOrRoute(point.coords);
                }
              }}
              type="start"
            />
            <LocationInput
              placeholder="Bitiş seç..."
              value={endPoint?.name}
              onSelect={setEndPoint}
              type="end"
            />
            <button
              onClick={handleNewRoute}
              className="w-full py-3 mt-6 rounded-lg text-white font-semibold bg-gradient-to-r from-orange-500 to-red-500 hover:from-orange-600 hover:to-red-600 transition"
            >
              + Yeni Rota
            </button>
          </div>

          {/* Sağdaki POI Listesi */}
          <div className="flex-1 bg-white p-6 rounded-xl shadow-md border overflow-y-auto">
            <h3 className="text-2xl font-semibold mb-4">Yakındaki POI’lar</h3>

            {/* ✅ Arama Çubuğu */}
            <input
              type="text"
              placeholder="POI ara..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-3 py-2 mb-4 border rounded-lg text-sm"
            />

            <ul className="space-y-3">
              {filteredPois.map((poi, idx) => (
                <li
                  key={poi.uniqueId || `${poi.id}-${idx}`}
                  className="flex justify-between items-center text-gray-700 border-b pb-2 cursor-pointer"
                  onClick={() => handlePoiClick(poi)}
                >
                  <div>
                    <span className="font-bold">{poi.name}</span>
                    <span className="block text-xs text-gray-500">
                      {poi.category} • ⭐ {poi.avgRating?.toFixed(1)}
                    </span>
                  </div>
                  <button className="text-gray-500 hover:text-blue-600">
                    <FaPlus />
                  </button>
                </li>
              ))}
              {filteredPois.length === 0 && (
                <p className="text-gray-400 text-sm">Sonuç bulunamadı.</p>
              )}
            </ul>

            {selectedPoi && (
              <div className="mt-6 p-4 border rounded-lg bg-gray-50">
                <h4 className="font-bold text-lg mb-2">{selectedPoi.name}</h4>
                <p className="text-sm text-gray-600 mb-1">
                  Kategori: {selectedPoi.category}
                </p>
                <p className="text-sm text-gray-600 mb-1">
                  Ortalama Puan: ⭐ {selectedPoi.avgRating?.toFixed(1) || "-"}
                </p>
                <p className="text-sm text-gray-600">
                  {selectedPoi.description || "Açıklama bulunmuyor"}
                </p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default MapComponentPage;
