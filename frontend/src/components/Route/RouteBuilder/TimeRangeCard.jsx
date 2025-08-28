// src/components/Route/RouteBuilder/TimeRangeCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { Slider } from 'primereact/slider';
import { Checkbox } from 'primereact/checkbox';

const TimeRangeCard = ({ timeRange, setTimeRange, flexibleTime, setFlexibleTime }) => {
    return (
        <Card className="shadow-md">
            <div className="p-6">
                <div className="flex items-center mb-4">
                    <i className="pi pi-clock text-blue-500 mr-2"></i>
                    <h2 className="text-xl font-semibold text-gray-900">
                        Zaman Aralığı
                    </h2>
                </div>
                
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-3">
                            Saat Aralığı: {timeRange[0]}:00 - {timeRange[1]}:00
                        </label>
                        <div className="px-3">
                            <Slider 
                                value={timeRange} 
                                onChange={(e) => setTimeRange(e.value)} 
                                range 
                                min={0} 
                                max={24}
                                className="w-full"
                            />
                        </div>
                        <div className="flex justify-between text-xs text-gray-500 mt-2">
                            <span>00:00</span>
                            <span>12:00</span>
                            <span>24:00</span>
                        </div>
                    </div>
                    
                    <div className="flex items-center">
                        <Checkbox 
                            inputId="flexible-time" 
                            checked={flexibleTime} 
                            onChange={(e) => setFlexibleTime(e.checked)} 
                        />
                        <label htmlFor="flexible-time" className="ml-2 text-sm text-gray-700">
                            Esnek zaman aralığı
                        </label>
                    </div>
                </div>
            </div>
        </Card>
    );
};

export default TimeRangeCard;