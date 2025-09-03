// src/pages/RoutesPage.jsx
import React, { useState, useEffect } from 'react';
import FilterSidebar from '../components/Route/FilterSidebar';
import RouteCard from '../components/Route/RouteCard';
import { getTravelRoutes, filterRoutes } from '../services/RouteFilterService';   // ✅ backend servisleri
import 'primereact/resources/themes/lara-light-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';

const RoutesPage = () => {
    const [filters, setFilters] = useState({
        sortOption: null,
        selectedCategory: null,
        ratingValue: 1.0,
        selectedDuration: null,
        distanceValue: 50
    });
    const [filteredRoutes, setFilteredRoutes] = useState([]);
    const [loading, setLoading] = useState(false);

    // Sayfa yüklendiğinde rotaları backend’den getir
    useEffect(() => {
        async function loadRoutes() {
            setLoading(true);
            try {
                const data = await getTravelRoutes();
                setFilteredRoutes(data);
                console.log("Backend'den gelen rotalar:", data);
            } catch (err) {
                console.error("Rotalar yüklenemedi:", err);
            } finally {
                setLoading(false);
            }
        }
        loadRoutes();
    }, []);

    const handleFilterChange = (filterName, value) => {
        setFilters(prevFilters => ({ ...prevFilters, [filterName]: value }));
    };

    // filtre uygula → backend'e gönder
    const handleApplyFilters = async () => {
        setLoading(true);
        try {
            const response = await filterRoutes(filters);   // ✅ backend’e istek
            console.log("Backend'den gelen rotalar:", response);
            setFilteredRoutes(response || []);
        } catch (error) {
            console.error("Filtreleme hatası:", error);
            setFilteredRoutes([]);
        } finally {
            setLoading(false);
        }
    };

    const handleResetFilters = async () => {
        setFilters({
            sortOption: null,
            selectedCategory: null,
            ratingValue: 1.0,
            selectedDuration: null,
            distanceValue: 50
        });
        try {
            const data = await getTravelRoutes();
            setFilteredRoutes(data);
        } catch (err) {
            console.error("Rotalar yüklenemedi:", err);
            setFilteredRoutes([]);
        }
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
                            {loading ? (
                                <p className="col-span-2 text-center text-gray-500 py-10">Yükleniyor...</p>
                            ) : filteredRoutes.length > 0 ? (
                                filteredRoutes.map(route => (
                                    <RouteCard key={route.id} route={route} />
                                ))
                            ) : (
                                <p className="col-span-2 text-center text-gray-500 py-10">
                                    Bu kriterlere uygun rota bulunamadı.
                                </p>
                            )}
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
};

export default RoutesPage;
