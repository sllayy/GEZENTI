import React, { useState } from 'react';
import { Dropdown } from 'primereact/dropdown';
import { RadioButton } from 'primereact/radiobutton';

const FilterSidebar = () => {
    const [sortOption, setSortOption] = useState(null);
    const [selectedCategory, setSelectedCategory] = useState('Tarih');

    const sortOptions = [
        { name: 'Popülerliğe Göre', code: 'POP' },
        { name: 'Yeniye Göre', code: 'NEW' },
        { name: 'Puana Göre', code: 'RATE' },
    ];

    const categories = [
        { name: 'Tarih', key: 'Tarih', count: 45 },
        { name: 'Sanat', key: 'Sanat', count: 32 },
        { name: 'Yemek', key: 'Yemek', count: 28 },
        { name: 'Doğa', key: 'Doğa', count: 36 },
    ];

    return (
        <aside className="lg:col-span-1 space-y-6">
            <div className="bg-white p-6 rounded-2xl shadow-sm">
                <h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4">
                    <i className="pi pi-sort-alt mr-2"></i>Sıralama
                </h3>
                <Dropdown value={sortOption} onChange={(e) => setSortOption(e.value)} options={sortOptions} optionLabel="name" 
                          placeholder="Sıralama türü seçin" className="w-full" />
            </div>
            <div className="bg-white p-6 rounded-2xl shadow-sm">
                <h3 className="text-lg font-semibold text-gray-800 flex items-center mb-4">
                    <i className="pi pi-filter mr-2"></i>Kategori Filtreleri
                </h3>
                <div className="space-y-4">
                    {categories.map((category) => (
                        <div key={category.key} className="flex items-center justify-between">
                            <div className="flex items-center">
                                <RadioButton inputId={category.key} name="category" value={category.name} 
                                             onChange={(e) => setSelectedCategory(e.value)} checked={selectedCategory === category.name} />
                                <label htmlFor={category.key} className="ml-2 text-gray-700">{category.name}</label>
                            </div>
                            <span className="text-sm bg-gray-100 text-gray-600 font-medium px-2 py-0.5 rounded-full">{category.count}</span>
                        </div>
                    ))}
                </div>
            </div>
        </aside>
    );
};

export default FilterSidebar;