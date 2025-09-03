// src/pages/RouteBuilderPage.jsx

import React, { useState, useRef } from 'react';
import { Toast } from 'primereact/toast';
import RouteInfoCard from '../components/Route/RouteBuilder/RouteInfoCard';
import CategorySelectorCard from '../components/Route/RouteBuilder/CategorySelectorCard';
import TimeRangeCard from '../components/Route/RouteBuilder/TimeRangeCard';
import TransportTypeCard from '../components/Route/RouteBuilder/TransportTypeCard';
import ExtraOptionsCard from '../components/Route/RouteBuilder/ExtraOptionsCard';
import RoutePreviewCard from '../components/Route/RouteBuilder/RoutePreviewCard';
import PersonalizedSuggestionsCard from '../components/Route/RouteBuilder/PersonalizedSuggestionsCard';
import { userPreferencesAPI, routeAPI } from '../services/api';

// API base URL (.env içinde REACT_APP_API_URL=http://localhost:5136/api)
const API_URL = process.env.REACT_APP_API_URL;

const RouteBuilderPage = () => {
    // --- State ---
    const [routeName, setRouteName] = useState('');
    const [startCoords, setStartCoords] = useState([28.976018, 41.00527]); // başlangıç
    const [endCoords, setEndCoords] = useState([28.970833, 41.010556]);   // bitiş
    const [selectedCategories, setSelectedCategories] = useState([]);
    const [isCreatingRoute, setIsCreatingRoute] = useState(false);
    const [timeRange, setTimeRange] = useState([8, 18]);
    const [flexibleTime, setFlexibleTime] = useState(false);
    const [transportType, setTransportType] = useState('walking');
    const [considerTraffic, setConsiderTraffic] = useState(false);
    const [shortestRoute, setShortestRoute] = useState(false);
    const [crowdPreference, setCrowdPreference] = useState(0);
    const [maxWalkDistance, setMaxWalkDistance] = useState(2000);
    const [minPoiRating, setMinPoiRating] = useState(0);

    const [routeData, setRouteData] = useState(null);

    const toast = useRef(null);

    // Kategoriler
    const categories = [
        { id: 'Tarih', label: 'Tarih', icon: 'pi pi-clock' },
        { id: 'Yemek', label: 'Yemek', icon: 'pi pi-star' },
        { id: 'Müzik', label: 'Müzik', icon: 'pi pi-volume-up' },
        { id: 'Müze', label: 'Müze', icon: 'pi pi-bolt' },
        { id: 'Alışveriş', label: 'Alışveriş', icon: 'pi pi-shopping-cart' },
        { id: 'Eğlence', label: 'Eğlence', icon: 'pi pi-moon' },
        { id: 'Sanat', label: 'Sanat', icon: 'pi pi-users' },
        { id: 'Sahil', label: 'Sahil', icon: 'pi pi-camera' },
        { id: 'Doğa', label: 'Doğa', icon: 'pi pi-leaf' }
    ];

    // Ulaşım türleri
    const transportTypes = [
        { id: 'walking', label: 'Yürüyüş', icon: 'pi pi-user', subtitle: 'Yaya olarak', speed: '~2-3 km/saat' },
        { id: 'car', label: 'Araç', icon: 'pi pi-car', subtitle: 'Özel araç', speed: '~40-60 km/saat' },
        { id: 'public', label: 'Toplu Taşıma', icon: 'pi pi-building', subtitle: 'Otobüs/Metro', speed: '~25-35 km/saat' }
    ];

    // Rota oluşturma
    const handleCreateRoute = async () => {
        if (!routeName.trim()) {
            toast.current.show({ severity: 'warn', summary: 'Uyarı', detail: 'Lütfen rota adını girin', life: 3000 });
            return;
        }

        if (selectedCategories.length === 0) {
            toast.current.show({ severity: 'warn', summary: 'Uyarı', detail: 'Lütfen en az bir kategori seçin', life: 3000 });
            return;
        }

        try {
            setIsCreatingRoute(true);

            // 🔹 Mevcut kullanıcıyı al (şimdilik localStorage)

            // --- UserPreferences backend’e kaydet ---
            const prefsPayload = {
                crowdednessPreference: crowdPreference,
                maxWalkDistance,
                preferredThemes: selectedCategories,
                preferredTransportationMode: transportType,
                minStartTimeHour: timeRange[0],
                maxEndTimeHour: timeRange[1],
                minPoiRating,
                considerTraffic,
                prioritizeShortestRoute: shortestRoute,
                accessibilityFriendly: false
            };
            await userPreferencesAPI.save(prefsPayload);

            // --- Rota isteği ---
            const payload = {
                
                mode: transportType === 'walking' ? 'foot' : 'driving',
                coordinates: [startCoords, endCoords],
                optimizeOrder: shortestRoute,
                returnToStart: false,
                alternatives: 0,
                geoJson: true,
                snapToNetwork: true,
                preference: shortestRoute ? 'shortest' : 'fastest',
                minStartTimeHour: timeRange[0],
                maxEndTimeHour: timeRange[1],
                totalAvailableMinutes: (timeRange[1] - timeRange[0]) * 60
            };

            const response = await routeAPI.optimize(payload);
            setRouteData(response);

            toast.current.show({ severity: 'success', summary: 'Başarılı', detail: 'Rotanız başarıyla oluşturuldu!', life: 3000 });
        } catch (error) {
            console.error('Rota oluşturma hatası:', error);
            toast.current.show({ severity: 'error', summary: 'Hata', detail: 'Rota oluşturulurken bir hata oluştu', life: 4000 });
        } finally {
            setIsCreatingRoute(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 py-8">
            <Toast ref={toast} />

            {/* Header */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mb-8">
                <div className="text-center">
                    <h1 className="text-4xl font-bold text-gray-900 mb-2">Kişiselleştirilmiş Rota Oluştur</h1>
                    <p className="text-lg text-gray-600">Tercihlerinize göre optimize edilmiş rotalar oluşturun</p>
                </div>
            </div>

            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="grid grid-cols-1 lg:grid-cols-5 gap-8">
                    {/* Sol Panel */}
                    <div className="space-y-6 lg:col-span-2">
                        <RouteInfoCard
                            routeName={routeName}
                            setRouteName={setRouteName}
                            startCoords={startCoords}
                            setStartCoords={setStartCoords}
                            endCoords={endCoords}
                            setEndCoords={setEndCoords}
                        />

                        <CategorySelectorCard
                            categories={categories}
                            selectedCategories={selectedCategories}
                            setSelectedCategories={setSelectedCategories}
                        />

                        <TimeRangeCard timeRange={timeRange} setTimeRange={setTimeRange} flexibleTime={flexibleTime} setFlexibleTime={setFlexibleTime} />

                        <TransportTypeCard transportTypes={transportTypes} transportType={transportType} setTransportType={setTransportType} />

                        <ExtraOptionsCard
                            considerTraffic={considerTraffic}
                            setConsiderTraffic={setConsiderTraffic}
                            shortestRoute={shortestRoute}
                            setShortestRoute={setShortestRoute}
                            crowdPreference={crowdPreference}
                            setCrowdPreference={setCrowdPreference}
                            maxWalkDistance={maxWalkDistance}
                            setMaxWalkDistance={setMaxWalkDistance}
                            minPoiRating={minPoiRating}
                            setMinPoiRating={setMinPoiRating}
                        />
                    </div>

                    {/* Sağ Panel */}
                    <div className="space-y-6 lg:col-span-3">
                        <RoutePreviewCard
                            isCreatingRoute={isCreatingRoute}
                            onCreateRoute={handleCreateRoute}
                            routeData={routeData}
                        />
                        <PersonalizedSuggestionsCard toast={toast} />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default RouteBuilderPage;
