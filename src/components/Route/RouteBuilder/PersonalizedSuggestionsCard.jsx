// src/components/Route/RouteBuilder/PersonalizedSuggestionsCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';

const PersonalizedSuggestionsCard = ({ toast }) => {
    const applyRoute = (routeName) => {
        toast.current.show({
            severity: 'success',
            summary: 'Rota Uygulandı',
            detail: `${routeName} uygulandı`,
            life: 3000
        });
    };

    return (
        <Card className="shadow-md">
            <div className="p-6">
                <div className="flex items-center mb-4">
                    <i className="pi pi-map-marker text-blue-500 mr-2"></i>
                    <h2 className="text-xl font-semibold text-gray-900">
                        Kişiselleştirilmiş Öneriler
                    </h2>
                </div>
                
                <div className="grid grid-cols-1 gap-4">
                    {/* Tarihi Odaklı Rota */}
                    <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                        <div className="flex justify-between items-start mb-3">
                            <h3 className="font-semibold text-gray-900">Tarihi Odaklı Rota</h3>
                            <span className="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full">
                                Yüksek
                            </span>
                        </div>
                        <p className="text-sm text-gray-600 mb-3">
                            Seçtiğiniz tarih kategorisine göre optimize edilmiş rota
                        </p>
                        <div className="flex items-center justify-between text-sm text-gray-500 mb-3">
                            <div className="flex items-center">
                                <i className="pi pi-clock mr-1"></i>
                                <span>4-6 saat</span>
                            </div>
                            <div className="flex items-center">
                                <i className="pi pi-map-marker mr-1"></i>
                                <span>8 POI</span>
                            </div>
                        </div>
                        <Button
                            label="Rotayı Uygula"
                            size="small"
                            className="w-full bg-green-500 hover:bg-green-600 border-0 text-white"
                            onClick={() => applyRoute('Tarihi Odaklı Rota')}
                        />
                    </div>

                    {/* Sanat & Kültür Rotası */}
                    <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                        <div className="flex justify-between items-start mb-3">
                            <h3 className="font-semibold text-gray-900">Sanat & Kültür Rotası</h3>
                            <span className="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
                                Orta
                            </span>
                        </div>
                        <p className="text-sm text-gray-600 mb-3">
                            Müzeler ve sanat galerilerini öncelleyen rota
                        </p>
                        <div className="flex items-center justify-between text-sm text-gray-500 mb-3">
                            <div className="flex items-center">
                                <i className="pi pi-clock mr-1"></i>
                                <span>3-5 saat</span>
                            </div>
                            <div className="flex items-center">
                                <i className="pi pi-map-marker mr-1"></i>
                                <span>6 POI</span>
                            </div>
                        </div>
                        <Button
                            label="Rotayı Uygula"
                            size="small"
                            className="w-full bg-blue-500 hover:bg-blue-600 border-0 text-white"
                            onClick={() => applyRoute('Sanat & Kültür Rotası')}
                        />
                    </div>

                    {/* Gastronomik Tur */}
                    <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                        <div className="flex justify-between items-start mb-3">
                            <h3 className="font-semibold text-gray-900">Gastronomik Tur</h3>
                            <span className="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full">
                                Orta
                            </span>
                        </div>
                        <p className="text-sm text-gray-600 mb-3">
                            Yerel lezzetler ve restoranlar odaklı rota
                        </p>
                        <div className="flex items-center justify-between text-sm text-gray-500 mb-3">
                            <div className="flex items-center">
                                <i className="pi pi-clock mr-1"></i>
                                <span>5-7 saat</span>
                            </div>
                            <div className="flex items-center">
                                <i className="pi pi-map-marker mr-1"></i>
                                <span>10 POI</span>
                            </div>
                        </div>
                        <Button
                            label="Rotayı Uygula"
                            size="small"
                            className="w-full bg-green-500 hover:bg-green-600 border-0 text-white"
                            onClick={() => applyRoute('Gastronomik Tur')}
                        />
                    </div>
                </div>
            </div>
        </Card>
    );
};

export default PersonalizedSuggestionsCard;