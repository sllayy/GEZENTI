import React, { useState } from 'react';
import FilterSidebar from '../components/Route/FilterSidebar';
import RouteCard from '../components/Route/RouteCard';

// 1. Her rota nesnesinden 'difficulty' özelliği kaldırıldı.
const allRoutes = [
    { id: 1, title: 'İstanbul Tarihi Yarımada Turu', rating: 4.8, distance: 8, durationCategory: 'CokGunluk', category: 'Tarih', imageUrl: 'https://...', tags: ['Tarih', 'Kültür'], author: { name: 'Ahmet Yılmaz' }, likes: 124, comments: 203 },
    { id: 2, title: 'Kapadokya Macera Rotası', rating: 4.9, distance: 25, durationCategory: 'CokGunluk', category: 'Doğa', imageUrl: 'https://...', tags: ['Doğa', 'Macera'], author: { name: 'Elif Kaya' }, likes: 250, comments: 150 },
    { id: 3, title: 'Ege Antik Kentler Turu', rating: 4.7, distance: 45, durationCategory: 'CokGunluk', category: 'Tarih', imageUrl: 'https://...', tags: ['Arkeoloji'], author: { name: 'Mehmet Özkan' }, likes: 45, comments: 87 },
    { id: 4, title: 'Likya Yolu Yürüyüşü', rating: 4.9, distance: 50, durationCategory: 'CokGunluk', category: 'Doğa', imageUrl: 'https://...', tags: ['Trekking'], author: { name: 'Ayşe Vural' }, likes: 500, comments: 312 },
    { id: 5, title: 'Ankara Gurme Turu', rating: 4.5, distance: 5, durationCategory: 'Orta', category: 'Yemek', imageUrl: 'https://...', tags: ['Yemek'], author: { name: 'Can Tekin' }, likes: 95, comments: 40 }
];

const RoutesPage = () => {
    // 2. 'filters' state'inden 'selectedDifficulty' kaldırıldı.
    const [filters, setFilters] = useState({
        sortOption: null,
        selectedCategory: null,
        ratingValue: 1.0,
        selectedDuration: null,
        distanceValue: 50
    });
    const [filteredRoutes, setFilteredRoutes] = useState(allRoutes);

    const handleFilterChange = (filterName, value) => {
        setFilters(prevFilters => ({ ...prevFilters, [filterName]: value }));
    };

    const handleApplyFilters = () => {
        let tempRoutes = [...allRoutes];
        
        // 3. Filtreleme mantığından zorluk seviyesi satırı kaldırıldı.
        tempRoutes = tempRoutes.filter(route => 
            route.rating >= filters.ratingValue &&
            route.distance <= filters.distanceValue &&
            (!filters.selectedCategory || route.category === filters.selectedCategory) &&
            (!filters.selectedDuration || route.durationCategory === filters.selectedDuration)
        );
        setFilteredRoutes(tempRoutes);
    };

    const handleResetFilters = () => {
        // 4. Sıfırlama işleminden 'selectedDifficulty' kaldırıldı.
        setFilters({
            sortOption: null, selectedCategory: null, ratingValue: 1.0,
            selectedDuration: null, distanceValue: 50
        });
        setFilteredRoutes(allRoutes);
    };
    
    return (
        <div className="bg-gray-50 min-h-screen">
            <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <main className="grid grid-cols-1 lg:grid-cols-4 gap-8">
                    <FilterSidebar
                        filters={filters}
                        onFilterChange={handleFilterChange}
                        onApply={handleApplyFilters}
                        onReset={handleResetFilters}
                    />
                    <div className="lg:col-span-3">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            {filteredRoutes.length > 0 ? (
                                filteredRoutes.map(route => <RouteCard key={route.id} route={route} />)
                            ) : (
                                <p className="col-span-2 text-center text-gray-500 py-10">Bu kriterlere uygun rota bulunamadı.</p>
                            )}
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
};

export default RoutesPage;