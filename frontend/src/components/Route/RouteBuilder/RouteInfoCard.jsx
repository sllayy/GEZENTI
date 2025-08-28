// src/components/Route/RouteBuilder/RouteInfoCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { InputText } from 'primereact/inputtext';

const RouteInfoCard = ({ routeName, setRouteName, startLocation, setStartLocation }) => {
    return (
        <Card className="shadow-md">
            <div className="p-6">
                <h2 className="text-xl font-semibold text-gray-900 mb-4">
                    Rota Bilgileri
                </h2>
                
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Rota Adı
                        </label>
                        <InputText
                            value={routeName}
                            onChange={(e) => setRouteName(e.target.value)}
                            placeholder="Örn: İstanbul Tarihi Turu"
                            className="w-full"
                        />
                    </div>
                    
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Başlangıç Noktası
                        </label>
                        <InputText
                            value={startLocation}
                            onChange={(e) => setStartLocation(e.target.value)}
                            className="w-full"
                        />
                    </div>
                </div>
            </div>
        </Card>
    );
};

export default RouteInfoCard;