// src/components/Route/RouteBuilder/RoutePreviewCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';

// Örnek bir rota verisi (Backend'den geleceğini varsayalım)
const dummyRouteData = {
    distance: "12 km",
    duration: "3 saat 30 dakika",
    poiCount: 7,
    stops: [
        { id: 1, name: "Galata Kulesi", time: "10:00 - 11:00", icon: "pi pi-map-marker" },
        { id: 2, name: "İstanbul Modern", time: "11:30 - 13:00", icon: "pi pi-palette" },
        { id: 3, name: "Taksim Meydanı", time: "13:30 - 14:00", icon: "pi pi-building" },
        { id: 4, name: "İstiklal Caddesi", time: "14:00 - 16:00", icon: "pi pi-shopping-bag" },
    ]
};

const RoutePreviewCard = ({ isCreatingRoute, onCreateRoute, onOptimizeRoute, routeData }) => {
    // Rota verisi yoksa varsayılan içeriği göster
    const isRouteGenerated = routeData && routeData.stops && routeData.stops.length > 0;

    const renderContent = () => {
        if (!isRouteGenerated) {
            return (
                <div className="text-center p-8 text-gray-400">
                    <p>Rota oluşturmak için sol paneldeki seçenekleri doldurun.</p>
                </div>
            );
        }

        return (
            <div>
                {/* Rota Özeti */}
                <div className="flex justify-around items-center mb-6 text-gray-600 font-medium">
                    <div className="flex items-center">
                        <i className="pi pi-map-marker-alt text-lg mr-2 text-blue-500"></i>
                        <span>{routeData.poiCount} POI</span>
                    </div>
                    <div className="flex items-center">
                        <i className="pi pi-clock text-lg mr-2 text-blue-500"></i>
                        <span>{routeData.duration}</span>
                    </div>
                    <div className="flex items-center">
                        <i className="pi pi-compass text-lg mr-2 text-blue-500"></i>
                        <span>{routeData.distance}</span>
                    </div>
                </div>

                {/* Harita Alanı */}
                <div className="bg-gray-200 h-64 rounded-lg flex items-center justify-center text-gray-500 mb-6">
                    [Harita bileşeni buraya gelecek]
                </div>
                
                {/* Duraklar Listesi */}
                <div className="space-y-4">
                    {routeData.stops.map((stop, index) => (
                        <div key={stop.id} className="flex items-center p-3 rounded-lg border">
                            <i className={`${stop.icon} text-blue-500 text-xl mr-4`}></i>
                            <div>
                                <h4 className="font-semibold text-gray-900">{stop.name}</h4>
                                <span className="text-sm text-gray-500">{stop.time}</span>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        );
    };

    return (
        <Card className="shadow-lg rounded-xl p-6">
            <h3 className="text-2xl font-bold mb-4">Rota Önizleme</h3>
            
            {renderContent()}

            <div className="flex flex-col space-y-4 mt-6">
                {/* Rota Optimize Et Butonu */}
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
                
                {/* Rota Oluştur Butonu */}
                <Button
                    label={isCreatingRoute ? 'Oluşturuluyor...' : 'Rotayı Oluştur'}
                    disabled={isCreatingRoute}
                    className={`
                        w-full py-3 px-4 rounded-xl text-white font-semibold
                        bg-gradient-to-r from-blue-600 to-cyan-600
                        ${isCreatingRoute ? 'opacity-50 cursor-not-allowed' : 'hover:from-blue-700 hover:to-cyan-700'}
                        transition-colors duration-300 border-0
                    `}
                    onClick={onCreateRoute}
                />
            </div>
        </Card>
    );
};

export default RoutePreviewCard;