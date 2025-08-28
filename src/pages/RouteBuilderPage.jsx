// src/pages/RouteBuilderPage.jsx

import React, { useState, useRef } from 'react';
import { Toast } from 'primereact/toast';
import { Divider } from 'primereact/divider'; // Kullanılmadığı için kaldırılabilir
import { Button } from 'primereact/button'; // Kullanılmadığı için kaldırılabilir
import { Card } from 'primereact/card'; // Kullanılmadığı için kaldırılabilir
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
    // const [accessibleRoute, setAccessibleRoute] = useState(false); // ExtraOptionsCard'dan kaldırıldığı için burada da kaldırıldı

    // YENİ EKLENEN: crowdPreference state'i
    const [crowdPreference, setCrowdPreference] = useState(0); // Başlangıç değeri 0 olarak ayarlandı

    const toast = useRef(null);

    // Kategori listesi (burada veya ayrı bir config dosyasında tutulabilir)
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

    // Ulaşım türleri (burada veya ayrı bir config dosyasında tutulabilir)
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
            // Burada rotayı oluşturduktan sonra gerekli yönlendirmeleri veya state güncellemelerini yapabilirsiniz.
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
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                    
                    {/* Sol Panel */}
                    <div className="space-y-6">
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
                            // accessibleRoute prop'u ExtraOptionsCard'dan kaldırıldığı için burada da kaldırıldı
                            crowdPreference={crowdPreference} // YENİ EKLENEN PROP
                            setCrowdPreference={setCrowdPreference} // YENİ EKLENEN PROP
                        />
                    </div>

                    {/* Sağ Panel */}
                    <div className="space-y-6">
                        {/* Rota Önizleme ve Oluşturma Butonları */}
                        <RoutePreviewCard 
                            isCreatingRoute={isCreatingRoute}
                            onCreateRoute={handleCreateRoute}
                            onOptimizeRoute={handleOptimizeRoute}
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