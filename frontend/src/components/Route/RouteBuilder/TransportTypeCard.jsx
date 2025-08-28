// src/components/Route/RouteBuilder/TransportTypeCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { RadioButton } from 'primereact/radiobutton';

const TransportTypeCard = ({ transportTypes, transportType, setTransportType }) => {
    return (
        <Card className="shadow-md">
            <div className="p-6">
                <div className="flex items-center mb-4">
                    <i className="pi pi-send text-blue-500 mr-2"></i>
                    <h2 className="text-xl font-semibold text-gray-900">
                        Ulaşım Türü
                    </h2>
                </div>
                <p className="text-sm text-gray-600 mb-4">
                    Rota optimizasyonu için ulaşım şeklinizi seçin
                </p>
                
                <div className="space-y-3">
                    {transportTypes.map((transport) => (
                        <div key={transport.id} className="flex items-center p-3 border rounded-lg hover:bg-gray-50 transition-colors">
                            <RadioButton 
                                inputId={transport.id} 
                                name="transport" 
                                value={transport.id} 
                                onChange={(e) => setTransportType(e.value)} 
                                checked={transportType === transport.id} 
                            />
                            <label htmlFor={transport.id} className="ml-3 flex-1 cursor-pointer">
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center">
                                        <i className={`${transport.icon} mr-2 text-lg`}></i>
                                        <div>
                                            <div className="font-medium text-gray-900">{transport.label}</div>
                                            <div className="text-sm text-gray-500">{transport.subtitle}</div>
                                        </div>
                                    </div>
                                    <div className="text-sm text-gray-500">
                                        {transport.speed}
                                    </div>
                                </div>
                            </label>
                        </div>
                    ))}
                </div>
            </div>
        </Card>
    );
};

export default TransportTypeCard;