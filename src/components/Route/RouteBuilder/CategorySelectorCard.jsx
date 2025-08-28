// src/components/Route/RouteBuilder/CategorySelectorCard.jsx

import React from 'react';
import { Card } from 'primereact/card';
import { Button } from 'primereact/button';

const CategorySelectorCard = ({ categories, selectedCategories, setSelectedCategories }) => {
    const toggleCategory = (categoryId) => {
        setSelectedCategories(prev => {
            if (prev.includes(categoryId)) {
                return prev.filter(id => id !== categoryId);
            } else {
                return [...prev, categoryId];
            }
        });
    };

    return (
        <Card className="shadow-md">
            <div className="p-6">
                <div className="flex items-center mb-4">
                    <i className="pi pi-tags text-blue-500 mr-2"></i>
                    <h2 className="text-xl font-semibold text-gray-900">
                        Kategori Seçimi
                    </h2>
                </div>
                <p className="text-sm text-gray-600 mb-4">
                    İlgi alanlarınızı seçin (sıralama öncelik belirler)
                </p>
                
                <div className="grid grid-cols-2 gap-3">
                    {categories.map((category) => (
                        <Button
                            key={category.id}
                            className={`p-3 text-left border-2 transition-all duration-200 ${
                                selectedCategories.includes(category.id)
                                    ? 'bg-blue-50 border-blue-500 text-blue-700'
                                    : 'bg-white border-gray-200 text-gray-700 hover:border-gray-300'
                            }`}
                            onClick={() => toggleCategory(category.id)}
                        >
                            <div className="flex items-center">
                                <i className={`${category.icon} mr-2`}></i>
                                <span className="font-medium">{category.label}</span>
                            </div>
                        </Button>
                    ))}
                </div>

                {selectedCategories.length > 0 && (
                    <div className="mt-4 p-3 bg-blue-50 rounded-lg">
                        <p className="text-sm text-blue-700">
                            <strong>Seçilen kategoriler:</strong> {' '}
                            {selectedCategories.map(id => 
                                categories.find(cat => cat.id === id)?.label
                            ).join(', ')}
                        </p>
                    </div>
                )}
            </div>
        </Card>
    );
};

export default CategorySelectorCard;