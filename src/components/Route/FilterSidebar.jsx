import React from 'react';
import { Dropdown } from 'primereact/dropdown';
import { RadioButton } from 'primereact/radiobutton';
import { Slider } from 'primereact/slider';
import RatingFilter from './RatingFilter';

const FilterSidebar = ({ filters, onFilterChange, onApply, onReset }) => {
    // 1. 'difficulties' veri dizisi tamamen silindi.
    const sortOptions = [ { name: 'Popülerliğe Göre', code: 'POP' }, { name: 'Yeniye Göre', code: 'NEW' }, { name: 'Puana Göre', code: 'RATE' }];
   const categories = [ 
        { name: 'Tarih', key: 'Tarih', count: 45 }, 
        { name: 'Müze', key: 'Müze', count: 25 },
        { name: 'Sanat', key: 'Sanat', count: 32 }, 
        { name: 'Yemek', key: 'Yemek', count: 28 }, 
        { name: 'Alışveriş', key: 'Alisveris', count: 19 },
        { name: 'Doğa', key: 'Doğa', count: 36 },
        { name: 'Eğlence', key: 'Eglence', count: 41 },
        { name: 'Müzik', key: 'Müzik', count: 15 },
        { name: 'Sahil', key: 'Sahil', count: 22 }
    ];
    const durations = [ { name: 'Kısa (2-4 saat)', key: 'Kisa', count: 23 }, { name: 'Orta (4-6 saat)', key: 'Orta', count: 31 }, { name: 'Uzun (6+ saat)', key: 'Uzun', count: 18 }, { name: 'Çok Günlük', key: 'CokGunluk', count: 12 }];

    return (
        <aside className="lg:col-span-1 flex flex-col space-y-6">
            <div className="bg-white p-6 rounded-2xl shadow-sm"><h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4"><i className="pi pi-sort-alt mr-2"></i>Sıralama</h3><Dropdown value={filters.sortOption} onChange={(e) => onFilterChange('sortOption', e.value)} options={sortOptions} optionLabel="name" placeholder="Sıralama türü seçin" className="w-full" /></div>
            <div className="bg-white p-6 rounded-2xl shadow-sm"><h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4"><i className="pi pi-filter mr-2"></i>Kategori Filtreleri</h3><div className="space-y-4">{categories.map((cat) => (<div key={cat.key} className="flex items-center justify-between"><div className="flex items-center"><RadioButton inputId={cat.key} name="category" value={cat.name} onChange={(e) => onFilterChange('selectedCategory', e.value)} checked={filters.selectedCategory === cat.name} /><label htmlFor={cat.key} className="ml-2 text-gray-700">{cat.name}</label></div><span className="text-sm bg-gray-100 text-gray-600 font-medium px-2 py-0.5 rounded-full">{cat.count}</span></div>))}</div></div>
            <RatingFilter 
                value={filters.ratingValue} 
                onChange={(value) => onFilterChange('ratingValue', value)}
            />
            <div className="bg-white p-6 rounded-2xl shadow-sm"><h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4"><i className="pi pi-clock mr-2"></i>Süre Filtreleri</h3><div className="space-y-4">{durations.map((dur) => (<div key={dur.key} className="flex items-center justify-between"><div className="flex items-center"><RadioButton inputId={dur.key} name="duration" value={dur.key} onChange={(e) => onFilterChange('selectedDuration', e.value)} checked={filters.selectedDuration === dur.key} /><label htmlFor={dur.key} className="ml-2 text-gray-700">{dur.name}</label></div><span className="text-sm bg-gray-100 text-gray-600 font-medium px-2 py-0.5 rounded-full">{dur.count}</span></div>))}</div></div>
            

            <div className="bg-white p-6 rounded-2xl shadow-sm"><h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4"><i className="pi pi-map-marker mr-2"></i>Mesafe (km)</h3><p className="text-sm text-gray-600 mb-3">Maksimum Mesafe: {filters.distanceValue} km</p><Slider value={filters.distanceValue} onChange={(e) => onFilterChange('distanceValue', e.value)} min={1} max={50} className="w-full" /><div className="flex justify-between text-xs text-gray-500 mt-1"><span>1 km</span><span>50+ km</span></div></div>
            <div className="space-y-3">
                <button onClick={onApply} className="w-full flex justify-center items-center py-3 px-4 font-semibold text-white rounded-xl bg-gradient-to-r from-cyan-500 to-orange-400 hover:opacity-90 shadow-md">
                    <i className="pi pi-check mr-2"></i>Filtreleri Uygula
                </button>
                <button onClick={onReset} className="w-full flex justify-center items-center py-3 px-4 font-semibold text-gray-700 bg-white border border-gray-300 rounded-xl hover:bg-gray-100">
                    <i className="pi pi-refresh mr-2"></i>Filtreleri Sıfırla
                </button>
            </div>
        </aside>
    );
};

export default FilterSidebar;