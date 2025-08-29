import React from 'react';
import { Slider } from 'primereact/slider';

const RatingFilter = ({ value, onChange }) => {
    return (
        <div className="bg-white p-6 rounded-2xl shadow-sm">
            <h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4">
                <i className="pi pi-star mr-2 text-blue-500"></i>
                Puan Aralığı
            </h3>
            <p className="text-sm text-gray-600 mb-3">
                Minimum Puan: {value.toFixed(1)}
            </p>
            <Slider 
                value={value} 
                onChange={(e) => onChange(e.value)} 
                min={1.0} 
                max={5.0} 
                step={0.1} 
                className="w-full" 
            />
            <div className="flex justify-between text-xs text-gray-500 mt-1">
                <span>1.0★</span>
                <span>5.0★</span>
            </div>
        </div>
    );
};

export default RatingFilter;