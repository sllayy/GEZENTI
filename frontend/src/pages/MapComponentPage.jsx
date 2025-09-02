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
import { routeAPI } from "../services/api";

const MapComponent = () => {
    const mapElement = useRef(null);
    const [map, setMap] = useState(null);

    const [startPoint, setStartPoint] = useState(null);
    const [endPoint, setEndPoint] = useState(null);

    const [pois, setPois] = useState([]);
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

    // Marker & POI
    useEffect(() => {
        if (map) {
            console.log("poi ve rota page try → marker güncelleme başladı");
            markerSource.clear();

            if (startPoint) {
                const f = new Feature({
                    geometry: new Point(fromLonLat(startPoint.coords)),
                });
                console.log("poi ve rota page try → start marker çizildi", startPoint);
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
                console.log("poi ve rota page try → end marker çizildi", endPoint);
                f.setStyle(
                    new Style({
                        image: new Icon({
                            src: "https://cdn-icons-png.flaticon.com/512/684/684908.png",
                            color: "#008000",
                            scale: 0.07,
                        }),
                    })
                );
                markerSource.addFeature(f);
            }

            pois.forEach((poi) => {
                const poiFeature = new Feature({
                    geometry: new Point(fromLonLat([poi.longitude, poi.latitude])),
                });
                console.log("poi ve rota page try → POI marker çizildi", poi.name);
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
            console.log("poi ve rota page try → fetchPoisOrRoute çalıştı", { start, end });

            const body = { mode: "driving", coordinates: [start] };
            if (end) body.coordinates.push(end);

            const data = await routeAPI.getRouteWithPois(body);

            console.log("poi ve rota page try → backend response", data);

            setPois(data.visitPois || []);
            setRouteGeometry(data.geometry);

            routeSource.clear();

            if (end && data.geometry?.coordinates) {
                const coords = data.geometry.coordinates.map((c) =>
                    fromLonLat([c[0], c[1]])
                );
                const line = new Feature({ geometry: new LineString(coords) });
                routeSource.addFeature(line);
                console.log("poi ve rota page try → rota çizildi");
            }

            // zoom fit
            const extentPoints = [];
            if (start) extentPoints.push(fromLonLat(start));
            if (end) extentPoints.push(fromLonLat(end));
            (data.visitPois || []).forEach((poi) => {
                extentPoints.push(fromLonLat([poi.longitude, poi.latitude]));
            });

            if (extentPoints.length > 0 && map) {
                const extent = extentPoints.reduce(
                    (acc, curr) => [
                        Math.min(acc[0], curr[0]),
                        Math.min(acc[1], curr[1]),
                        Math.max(acc[2], curr[0]),
                        Math.max(acc[3], curr[1]),
                    ],
                    [Infinity, Infinity, -Infinity, -Infinity]
                );
                map.getView().fit(extent, { padding: [10, 10, 10, 10], duration: 800 });
                console.log("poi ve rota page try → zoom ayarlandı");
            }
        } catch (err) {
            console.error("poi ve rota page try → hata:", err);
        }
    };

    // İlk açılış
    useEffect(() => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (pos) => {
                    const coords = [pos.coords.longitude, pos.coords.latitude];
                    setStartPoint({ coords, name: "Mevcut Konum" });
                    fetchPoisOrRoute(coords);
                    console.log("poi ve rota page try → ilk açılış success");
                },
                () => {
                    console.warn("poi ve rota page try → konum izni yok, default İstanbul");
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
                <div className="flex flex-col md:flex-row gap-8 w-full max-w-4xl">
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

                    <div className="flex-1 bg-white p-6 rounded-xl shadow-md border">
                        <h3 className="text-2xl font-semibold mb-4">Yakındaki POI’lar</h3>
                        <ul className="space-y-3">
                            {pois.map((poi) => (
                                <li
                                    key={poi.id}
                                    className="flex justify-between items-center text-gray-700 border-b pb-2"
                                >
                                    <div>
                                        <span className="font-bold">{poi.name}</span>
                                        <span className="text-xs text-gray-500">
                                            {poi.category} • ⭐ {poi.avgRating?.toFixed(1)}
                                        </span>
                                    </div>
                                    <button className="text-gray-500 hover:text-blue-600">
                                        <FaPlus />
                                    </button>
                                </li>
                            ))}
                            {pois.length === 0 && (
                                <p className="text-gray-400 text-sm">Henüz POI bulunamadı.</p>
                            )}
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default MapComponent;
