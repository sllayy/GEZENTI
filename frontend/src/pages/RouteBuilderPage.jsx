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

const RouteBuilderPage = () => {
    // Tüm ana state'ler burada kalır
    const [routeName, setRouteName] = useState('');
    const [startLocation, setStartLocation] = useState('İstanbul Tarihi Turu');
    const [selectedCategories, setSelectedCategories] = useState([]);
    const [isCreatingRoute, setIsCreatingRoute] = useState(false);
    const [timeRange, setTimeRange] = useState([8, 18]);
    const [flexibleTime, setFlexibleTime] = useState(false);
    const [transportType, setTransportType] = useState('walking');
    const [considerTraffic, setConsiderTraffic] = useState(false);
    const [shortestRoute, setShortestRoute] = useState(false);
    const [crowdPreference, setCrowdPreference] = useState(0); 

    // YENİ EKLENEN: Rota verisi state'i
    const [routeData, setRouteData] = useState(null);

    const toast = useRef(null);

    // Kategori listesi
    const categories = [
        { id: 'tarih', label: 'Tarih', icon: 'pi pi-clock' },
        { id: 'yemek', label: 'Yemek', icon: 'pi pi-star' },
        { id: 'muzik', label: 'Müzik', icon: 'pi pi-volume-up' },
        { id: 'muze', label: 'Müze', icon: 'pi pi-bolt' },
        { id: 'alisveris', label: 'Alışveriş', icon: 'pi pi-shopping-cart' },
        { id: 'eglence', label: 'Eğlence', icon: 'pi pi-moon' },
        { id: 'sanat', label: 'Sanat', icon: 'pi pi-users' },
        { id: 'sahil', label: 'Sahil', icon: 'pi pi-camera' },
        { id: 'doga', label: 'Doga', icon: 'pi pi-camera' }
    ];

    // Ulaşım türleri
    const transportTypes = [
        { id: 'walking', label: 'Yürüyüş', icon: 'pi pi-user', subtitle: 'Yaya olarak', speed: '~2-3 km/saat' },
        { id: 'car', label: 'Araç', icon: 'pi pi-car', subtitle: 'Özel araç', speed: '~40-60 km/saat' },
        { id: 'public', label: 'Toplu Taşıma', icon: 'pi pi-building', subtitle: 'Otobüs/Metro', speed: '~25-35 km/saat' }
    ];

    // Rota oluşturma fonksiyonu (ana sayfada kalır)
    const handleCreateRoute = () => {
        if (!routeName.trim()) {
            toast.current.show({
                severity: 'warn',
                summary: 'Uyarı',
                detail: 'Lütfen rota adını girin',
                life: 3000
            });
            return;
        }

        if (selectedCategories.length === 0) {
            toast.current.show({
                severity: 'warn',
                summary: 'Uyarı',
                detail: 'Lütfen en az bir kategori seçin',
                life: 3000
            });
            return;
        }

        setIsCreatingRoute(true);
        // Simüle edilmiş API çağrısı
        setTimeout(() => {
            setIsCreatingRoute(false);
            toast.current.show({
                severity: 'success',
                summary: 'Başarılı',
                detail: 'Rotanız başarıyla oluşturuldu!',
                life: 3000
            });
            
            // ÖRNEK: Backend'den dönecek mockup verisi
            const mockData = {
                distance: "12 km",
                duration: "3 saat 30 dakika",
                poiCount: 7,
                stops: [
                    { id: 1, name: "Galata Kulesi", time: "10:00 - 11:00", icon: "pi pi-map-marker" },
                    { id: 2, name: "İstanbul Modern", time: "11:30 - 13:00", icon: "pi pi-palette" },
                    { id: 3, name: "Taksim Meydanı", time: "13:30 - 14:00", icon: "pi pi-building" },
                    { id: 4, name: "İstiklal Caddesi", time: "14:00 - 16:00", icon: "pi pi-shopping-bag" },
                ]
            };
            setRouteData(mockData); // Rota verisini state'e kaydediyoruz
        }, 2000);
    };

    const handleOptimizeRoute = () => {
        toast.current.show({
            severity: 'info',
            summary: 'Rota Optimizasyonu',
            detail: 'Rota optimizasyon işlemi başlatıldı',
            life: 3000
        });
    }

    return (
        <div className="min-h-screen bg-gray-50 py-8">
            <Toast ref={toast} />
            
            {/* Header */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mb-8">
                <div className="text-center">
                    <h1 className="text-4xl font-bold text-gray-900 mb-2">
                        Kişiselleştirilmiş Rota Oluştur
                    </h1>
                    <p className="text-lg text-gray-600">
                        Tercihlerinize göre optimize edilmiş rotalar oluşturun
                    </p>
                </div>
            </div>

            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                {/* Buradaki grid yapısını 2/3 oranında olacak şekilde değiştirdik */}
                <div className="grid grid-cols-1 lg:grid-cols-5 gap-8">
                    
                    {/* Sol Panel: 5 birimin 2'sini kaplar */}
                    <div className="space-y-6 lg:col-span-2">
                        {/* Rota Bilgileri */}
                        <RouteInfoCard 
                            routeName={routeName} 
                            setRouteName={setRouteName} 
                            startLocation={startLocation} 
                            setStartLocation={setStartLocation} 
                        />

                        {/* Kategori Seçimi */}
                        <CategorySelectorCard 
                            categories={categories}
                            selectedCategories={selectedCategories} 
                            setSelectedCategories={setSelectedCategories} 
                        />

                        {/* Zaman Aralığı */}
                        <TimeRangeCard 
                            timeRange={timeRange} 
                            setTimeRange={setTimeRange} 
                            flexibleTime={flexibleTime} 
                            setFlexibleTime={setFlexibleTime} 
                        />

                        {/* Ulaşım Türü */}
                        <TransportTypeCard 
                            transportTypes={transportTypes}
                            transportType={transportType} 
                            setTransportType={setTransportType} 
                        />

                        {/* Ek Seçenekler */}
                        <ExtraOptionsCard 
                            considerTraffic={considerTraffic} 
                            setConsiderTraffic={setConsiderTraffic} 
                            shortestRoute={shortestRoute} 
                            setShortestRoute={setShortestRoute} 
                            crowdPreference={crowdPreference}
                            setCrowdPreference={setCrowdPreference}
                        />
                    </div>

                    {/* Sağ Panel: 5 birimin 3'ünü kaplar */}
                    <div className="space-y-6 lg:col-span-3">
                        {/* Rota Önizleme ve Oluşturma Butonları */}
                        <RoutePreviewCard 
                            isCreatingRoute={isCreatingRoute}
                            onCreateRoute={handleCreateRoute}
                            onOptimizeRoute={handleOptimizeRoute}
                            routeData={routeData} // YENİ EKLENEN PROP
                        />

                        {/* Kişiselleştirilmiş Öneriler */}
                        <PersonalizedSuggestionsCard toast={toast} />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default RouteBuilderPage;