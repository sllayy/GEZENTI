// src/components/Route/RouteBuilder/RoutePreviewCard.jsx

import React, { useEffect, useRef } from "react";
import { Card } from "primereact/card";
import { Button } from "primereact/button";
import Map from "ol/Map";
import View from "ol/View";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { OSM, Vector as VectorSource } from "ol/source";
import GeoJSON from "ol/format/GeoJSON";
import { Stroke, Style, Icon } from "ol/style";
import { fromLonLat } from "ol/proj";
import Feature from "ol/Feature";
import Point from "ol/geom/Point";
import Overlay from "ol/Overlay";

const RoutePreviewCard = ({ isCreatingRoute, onCreateRoute, onOptimizeRoute, routeData }) => {
  const mapRef = useRef(null);
  const popupRef = useRef(null);

  useEffect(() => {
    if (!routeData || !routeData.geometry) return;

    // --- Rota Çizgisi ---
    const routeSource = new VectorSource({
      features: new GeoJSON().readFeatures(routeData.geometry, {
        featureProjection: "EPSG:3857",
      }),
    });

    const routeLayer = new VectorLayer({
      source: routeSource,
      style: new Style({
        stroke: new Stroke({
          color: "#007bff",
          width: 4,
        }),
      }),
    });

    // --- Markerlar ---
    const markerSource = new VectorSource();

    (routeData.visitPois || []).forEach((poi) => {
      if (poi.longitude && poi.latitude) {
        const marker = new Feature({
          geometry: new Point(fromLonLat([poi.longitude, poi.latitude])),
          name: poi.name,
        });

        marker.setStyle(
          new Style({
            image: new Icon({
              src: "https://cdn-icons-png.flaticon.com/512/684/684908.png", // kırmızı pin
              scale: 0.05,
              anchor: [0.5, 1],
            }),
          })
        );

        markerSource.addFeature(marker);
      }
    });

    const markerLayer = new VectorLayer({
      source: markerSource,
    });

    // --- Harita ---
    const map = new Map({
      target: mapRef.current,
      layers: [new TileLayer({ source: new OSM() }), routeLayer, markerLayer],
      view: new View({
        center: fromLonLat([28.976018, 41.00527]),
        zoom: 13,
      }),
    });

    // --- Popup Overlay ---
    const popup = new Overlay({
      element: popupRef.current,
      positioning: "bottom-center",
      stopEvent: false,
      offset: [0, -15],
    });
    map.addOverlay(popup);

    // Marker click event
    map.on("singleclick", (evt) => {
      const feature = map.forEachFeatureAtPixel(evt.pixel, (feat) => feat);
      if (feature && feature.get("name")) {
        const coordinate = evt.coordinate;
        popup.setPosition(coordinate);
        popupRef.current.innerHTML = `
          <div class="bg-white p-2 rounded-lg shadow-md border text-sm font-medium text-gray-800">
            ${feature.get("name")}
          </div>
        `;
        popupRef.current.style.display = "block";
      } else {
        popupRef.current.style.display = "none";
      }
    });

    // Fit to route
    const extent = routeSource.getExtent();
    if (extent && extent[0] !== Infinity) {
      map.getView().fit(extent, { padding: [50, 50, 50, 50], duration: 1000 });
    }

    return () => map.setTarget(undefined);
  }, [routeData]);

  // Rota özeti + içerik
  const renderContent = () => {
    if (!routeData) {
      return (
        <div className="text-center p-8 text-gray-400">
          <p>Rota oluşturmak için sol paneldeki seçenekleri doldurun.</p>
        </div>
      );
    }

    const hours = Math.floor(routeData.duration / 3600);
    const minutes = Math.round((routeData.duration % 3600) / 60);

    return (
      <div>
        {/* Rota Özeti */}
        <div className="flex justify-around items-center mb-6 text-gray-600 font-medium">
          <div className="flex items-center">
            <i className="pi pi-map-marker-alt text-lg mr-2 text-blue-500"></i>
            <span>{routeData.visitPois?.length || 0} POI</span>
          </div>
          <div className="flex items-center">
            <i className="pi pi-clock text-lg mr-2 text-blue-500"></i>
            <span>
              {hours > 0 ? `${hours} sa ` : ""}
              {minutes} dk
            </span>
          </div>
          <div className="flex items-center">
            <i className="pi pi-compass text-lg mr-2 text-blue-500"></i>
            <span>{(routeData.distance / 1000).toFixed(1)} km</span>
          </div>
        </div>

        {/* Harita */}
        <div
          ref={mapRef}
          className="w-full h-64 rounded-lg border border-gray-200 mb-6 relative"
        >
          <div
            ref={popupRef}
            className="absolute z-10"
            style={{ display: "none" }}
          ></div>
        </div>

        {/* Durak Listesi (Sıradaki Rota) */}
        <div className="mt-6">
          <h4 className="text-lg font-semibold text-gray-800 mb-3">
            Sıradaki Duraklar
          </h4>
          <ol className="space-y-3">
            {(routeData.visitPois || []).map((poi, index) => (
              <li
                key={index}
                className="flex items-center p-3 rounded-lg border bg-white shadow-sm"
              >
                <div className="flex items-center justify-center w-8 h-8 rounded-full bg-blue-500 text-white font-bold mr-4">
                  {index + 1}
                </div>
                <div>
                  <h5 className="font-semibold text-gray-900">{poi.name}</h5>
                  <span className="text-sm text-gray-500">
                    {poi.latitude}, {poi.longitude}
                  </span>
                </div>
              </li>
            ))}
          </ol>
        </div>
      </div>
    );
  };

  return (
    <Card className="shadow-lg rounded-xl p-6">
      <h3 className="text-2xl font-bold mb-4">Rota Önizleme</h3>

      {renderContent()}

      <div className="flex flex-col space-y-4 mt-6">
        <Button
          label="Rotayı Optimize Et"
          className="
              w-full py-3 px-4 rounded-xl text-white font-semibold
              bg-gradient-to-r from-orange-500 to-yellow-500
              hover:from-orange-600 hover:to-yellow-600
              transition-colors duration-300 border-0
          "
          onClick={onOptimizeRoute}
        />
        <Button
          label={isCreatingRoute ? "Oluşturuluyor..." : "Rotayı Oluştur"}
          disabled={isCreatingRoute}
          className={`
              w-full py-3 px-4 rounded-xl text-white font-semibold
              bg-gradient-to-r from-blue-600 to-cyan-600
              ${
                isCreatingRoute
                  ? "opacity-50 cursor-not-allowed"
                  : "hover:from-blue-700 hover:to-cyan-700"
              }
              transition-colors duration-300 border-0
          `}
          onClick={onCreateRoute}
        />
      </div>
    </Card>
  );
};

export default RoutePreviewCard;
