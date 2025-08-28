// src/components/Route/RouteBuilder/RoutePreviewCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';
import { Divider } from 'primereact/divider'; // Divider'ı da ekleyelim

const RoutePreviewCard = ({ isCreatingRoute, onCreateRoute, onOptimizeRoute }) => {
    return (
        <Card className="shadow-md">
            <div className="p-6">
                <div className="text-center mb-6">
                    <i className="pi pi-map text-4xl text-gray-400 mb-4"></i>
                    <h2 className="text-xl font-semibold text-gray-900 mb-2">
                        Rota Önizleme
                    </h2>
                    <p className="text-gray-600 mb-6">
                        Kategorileri seçtikten sonra rotanız burada görünecek
                    </p>
                </div>

                <div className="bg-gray-100 rounded-lg p-8 flex items-center justify-center min-h-48 mb-6">
                    <div className="text-center">
                        <i className="pi pi-map text-6xl text-gray-300 mb-4"></i>
                        <p className="text-gray-500">
                            Harita ve rota detayları burada gösterilecek
                        </p>
                    </div>
                </div>

                <Button
                    label="Rotayı Optimize Et"
                    icon="pi pi-cog"
                    className="w-full bg-gradient-to-r from-teal-400 to-orange-400 hover:from-teal-500 hover:to-orange-500 border-0 text-white font-semibold py-3 mb-4"
                    onClick={onOptimizeRoute}
                />
                
                <Divider />
                
                <Button
                    label={isCreatingRoute ? "Rota Oluşturuluyor..." : "Rotayı Oluştur"}
                    icon={isCreatingRoute ? "pi pi-spin pi-spinner" : "pi pi-plus"}
                    className="w-full bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 border-0 text-white font-semibold py-3"
                    onClick={onCreateRoute}
                    disabled={isCreatingRoute}
                />
            </div>
        </Card>
    );
};

export default RoutePreviewCard;