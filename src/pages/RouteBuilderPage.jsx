import React, { useState, useRef } from 'react';
import { InputText } from 'primereact/inputtext';
import { Button } from 'primereact/button';
import { Card } from 'primereact/card';
import { Divider } from 'primereact/divider';
import { Toast } from 'primereact/toast';
import { ProgressSpinner } from 'primereact/progressspinner';
import { Slider } from 'primereact/slider';
import { RadioButton } from 'primereact/radiobutton';
import { Checkbox } from 'primereact/checkbox';

const RouteBuilder = () => {
    const [routeName, setRouteName] = useState('');
    const [startLocation, setStartLocation] = useState('İstanbul Tarihi Turu');
    const [selectedCategories, setSelectedCategories] = useState([]);
    const [isCreatingRoute, setIsCreatingRoute] = useState(false);
    
    // Yeni state'ler
    const [timeRange, setTimeRange] = useState([8, 18]);
    const [flexibleTime, setFlexibleTime] = useState(false);
    const [transportType, setTransportType] = useState('walking');
    const [considerTraffic, setConsiderTraffic] = useState(false);
    const [shortestRoute, setShortestRoute] = useState(false);
    const [accessibleRoute, setAccessibleRoute] = useState(false);
    
    const toast = useRef(null);

    // Kategori listesi
    const categories = [
        { id: 'tarih', label: 'Tarih', icon: 'pi pi-clock' },
        { id: 'yemek', label: 'Yemek', icon: 'pi pi-star' },
        { id: 'muzik', label: 'Müzik', icon: 'pi pi-volume-up' },
        { id: 'spor', label: 'Spor', icon: 'pi pi-bolt' },
        { id: 'alisveris', label: 'Alışveriş', icon: 'pi pi-shopping-cart' },
        { id: 'gece-hayati', label: 'Gece Hayatı', icon: 'pi pi-moon' },
        { id: 'aile', label: 'Aile', icon: 'pi pi-users' },
        { id: 'fotograf', label: 'Fotoğraf', icon: 'pi pi-camera' }
    ];

    // Ulaşım türleri
    const transportTypes = [
        { id: 'walking', label: 'Yürüyüş', icon: 'pi pi-user', subtitle: 'Yaya olarak', speed: '~2-3 km/saat' },
        { id: 'bicycle', label: 'Bisiklet', icon: 'pi pi-circle', subtitle: 'Bisiklet ile', speed: '~15-20 km/saat' },
        { id: 'car', label: 'Araç', icon: 'pi pi-car', subtitle: 'Özel araç', speed: '~40-60 km/saat' },
        { id: 'public', label: 'Toplu Taşıma', icon: 'pi pi-building', subtitle: 'Otobüs/Metro', speed: '~25-35 km/saat' }
    ];

    // Kategori seçimi toggle fonksiyonu
    const toggleCategory = (categoryId) => {
        setSelectedCategories(prev => {
            if (prev.includes(categoryId)) {
                return prev.filter(id => id !== categoryId);
            } else {
                return [...prev, categoryId];
            }
        });
    };

    // Rota oluşturma fonksiyonu
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
        }, 2000);
    };

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
                    
                    {/* Sol Panel - Rota Bilgileri */}
                    <div className="space-y-6">
                        
                        {/* Rota Bilgileri Card */}
                        <Card className="shadow-md">
                            <div className="p-6">
                                <h2 className="text-xl font-semibold text-gray-900 mb-4">
                                    Rota Bilgileri
                                </h2>
                                
                                <div className="space-y-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Rota Adı
                                        </label>
                                        <InputText
                                            value={routeName}
                                            onChange={(e) => setRouteName(e.target.value)}
                                            placeholder="Örn: İstanbul Tarihi Turu"
                                            className="w-full"
                                        />
                                    </div>
                                    
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Başlangıç Noktası
                                        </label>
                                        <InputText
                                            value={startLocation}
                                            onChange={(e) => setStartLocation(e.target.value)}
                                            className="w-full"
                                        />
                                    </div>
                                </div>
                            </div>
                        </Card>

                        {/* Kategori Seçimi Card */}
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

                        {/* Zaman Aralığı Card */}
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

                        {/* Ulaşım Türü Card */}
                        <Card className="shadow-md">
                            <div className="p-6">
                                <div className="flex items-center mb-4">
                                    <i className="pi pi-send text-blue-500 mr-2"></i>
                                    <h2 className="text-xl font-semibold text-gray-900">
                                        Ulaşım Türü
                                    </h2>
                                </div>
                                <p className="text-sm text-gray-600 mb-4">
                                    Rota optimizasyonu için ulaşım şeklinizi seçin
                                </p>
                                
                                <div className="space-y-3">
                                    {transportTypes.map((transport) => (
                                        <div key={transport.id} className="flex items-center p-3 border rounded-lg hover:bg-gray-50 transition-colors">
                                            <RadioButton 
                                                inputId={transport.id} 
                                                name="transport" 
                                                value={transport.id} 
                                                onChange={(e) => setTransportType(e.value)} 
                                                checked={transportType === transport.id} 
                                            />
                                            <label htmlFor={transport.id} className="ml-3 flex-1 cursor-pointer">
                                                <div className="flex items-center justify-between">
                                                    <div className="flex items-center">
                                                        <i className={`${transport.icon} mr-2 text-lg`}></i>
                                                        <div>
                                                            <div className="font-medium text-gray-900">{transport.label}</div>
                                                            <div className="text-sm text-gray-500">{transport.subtitle}</div>
                                                        </div>
                                                    </div>
                                                    <div className="text-sm text-gray-500">
                                                        {transport.speed}
                                                    </div>
                                                </div>
                                            </label>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </Card>

                        {/* Ek Seçenekler Card */}
                        <Card className="shadow-md">
                            <div className="p-6">
                                <div className="flex items-center mb-4">
                                    <i className="pi pi-cog text-blue-500 mr-2"></i>
                                    <h2 className="text-xl font-semibold text-gray-900">
                                        Ek Seçenekler
                                    </h2>
                                </div>
                                
                                <div className="space-y-3">
                                    <div className="flex items-center justify-between p-3 border rounded-lg">
                                        <div className="flex items-center">
                                            <i className="pi pi-exclamation-triangle mr-3 text-orange-500"></i>
                                            <span className="font-medium">Trafik durumunu dikkate al</span>
                                        </div>
                                        <Checkbox 
                                            checked={considerTraffic} 
                                            onChange={(e) => setConsiderTraffic(e.checked)} 
                                        />
                                    </div>
                                    
                                    <div className="flex items-center justify-between p-3 border rounded-lg">
                                        <div className="flex items-center">
                                            <i className="pi pi-map-marker mr-3 text-green-500"></i>
                                            <span className="font-medium">En kısa rotayı öncelikle</span>
                                        </div>
                                        <Checkbox 
                                            checked={shortestRoute} 
                                            onChange={(e) => setShortestRoute(e.checked)} 
                                        />
                                    </div>
                                    
                                    <div className="flex items-center justify-between p-3 border rounded-lg">
                                        <div className="flex items-center">
                                            <i className="pi pi-wheelchair mr-3 text-blue-500"></i>
                                            <span className="font-medium">Engelli erişimi</span>
                                        </div>
                                        <Checkbox 
                                            checked={accessibleRoute} 
                                            onChange={(e) => setAccessibleRoute(e.checked)} 
                                        />
                                    </div>
                                </div>
                            </div>
                        </Card>
                    </div>

                    {/* Sağ Panel - Rota Önizleme */}
                    <div className="space-y-6">
                        {/* Rota Optimizasyon Butonu */}
                        <Card className="shadow-md">
                            <div className="p-6">
                                <div className="text-center mb-6">
                                    <i className="pi pi-map text-4xl text-gray-400 mb-4"></i>
                                    <h2 className="text-xl font-semibold text-gray-900 mb-2">
                                        Rota Önizleme
                                    </h2>
                                    <p className="text-gray-600 mb-6">
                                        Kategorileri seçtikten sonra rotanız burada görünecek
                                    </p>
                                </div>

                                <div className="bg-gray-100 rounded-lg p-8 flex items-center justify-center min-h-48 mb-6">
                                    <div className="text-center">
                                        <i className="pi pi-map text-6xl text-gray-300 mb-4"></i>
                                        <p className="text-gray-500">
                                            Harita ve rota detayları burada gösterilecek
                                        </p>
                                    </div>
                                </div>

                                <Button
                                    label="Rotayı Optimize Et"
                                    icon="pi pi-cog"
                                    className="w-full bg-gradient-to-r from-teal-400 to-orange-400 hover:from-teal-500 hover:to-orange-500 border-0 text-white font-semibold py-3 mb-4"
                                    onClick={() => {
                                        toast.current.show({
                                            severity: 'info',
                                            summary: 'Rota Optimizasyonu',
                                            detail: 'Rota optimizasyon işlemi başlatıldı',
                                            life: 3000
                                        });
                                    }}
                                />
                                
                                <Divider />
                                
                                <Button
                                    label={isCreatingRoute ? "Rota Oluşturuluyor..." : "Rotayı Oluştur"}
                                    icon={isCreatingRoute ? "pi pi-spin pi-spinner" : "pi pi-plus"}
                                    className="w-full bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 border-0 text-white font-semibold py-3"
                                    onClick={handleCreateRoute}
                                    disabled={isCreatingRoute}
                                />
                            </div>
                        </Card>

                        {/* Kişiselleştirilmiş Öneriler */}
                        <Card className="shadow-md">
                            <div className="p-6">
                                <div className="flex items-center mb-4">
                                    <i className="pi pi-map-marker text-blue-500 mr-2"></i>
                                    <h2 className="text-xl font-semibold text-gray-900">
                                        Kişiselleştirilmiş Öneriler
                                    </h2>
                                </div>
                                
                                <div className="grid grid-cols-1 gap-4">
                                    {/* Tarihi Odaklı Rota */}
                                    <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                                        <div className="flex justify-between items-start mb-3">
                                            <h3 className="font-semibold text-gray-900">Tarihi Odaklı Rota</h3>
                                            <span className="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full">
                                                Yüksek
                                            </span>
                                        </div>
                                        <p className="text-sm text-gray-600 mb-3">
                                            Seçtiğiniz tarih kategorisine göre optimize edilmiş rota
                                        </p>
                                        <div className="flex items-center justify-between text-sm text-gray-500 mb-3">
                                            <div className="flex items-center">
                                                <i className="pi pi-clock mr-1"></i>
                                                <span>4-6 saat</span>
                                            </div>
                                            <div className="flex items-center">
                                                <i className="pi pi-map-marker mr-1"></i>
                                                <span>8 POI</span>
                                            </div>
                                        </div>
                                        <Button
                                            label="Rotayı Uygula"
                                            size="small"
                                            className="w-full bg-green-500 hover:bg-green-600 border-0 text-white"
                                            onClick={() => {
                                                toast.current.show({
                                                    severity: 'success',
                                                    summary: 'Rota Uygulandı',
                                                    detail: 'Tarihi Odaklı Rota uygulandı',
                                                    life: 3000
                                                });
                                            }}
                                        />
                                    </div>

                                    {/* Sanat & Kültür Rotası */}
                                    <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                                        <div className="flex justify-between items-start mb-3">
                                            <h3 className="font-semibold text-gray-900">Sanat & Kültür Rotası</h3>
                                            <span className="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
                                                Orta
                                            </span>
                                        </div>
                                        <p className="text-sm text-gray-600 mb-3">
                                            Müzeler ve sanat galerilerini öncelleyen rota
                                        </p>
                                        <div className="flex items-center justify-between text-sm text-gray-500 mb-3">
                                            <div className="flex items-center">
                                                <i className="pi pi-clock mr-1"></i>
                                                <span>3-5 saat</span>
                                            </div>
                                            <div className="flex items-center">
                                                <i className="pi pi-map-marker mr-1"></i>
                                                <span>6 POI</span>
                                            </div>
                                        </div>
                                        <Button
                                            label="Rotayı Uygula"
                                            size="small"
                                            className="w-full bg-blue-500 hover:bg-blue-600 border-0 text-white"
                                            onClick={() => {
                                                toast.current.show({
                                                    severity: 'success',
                                                    summary: 'Rota Uygulandı',
                                                    detail: 'Sanat & Kültür Rotası uygulandı',
                                                    life: 3000
                                                });
                                            }}
                                        />
                                    </div>

                                    {/* Gastronomik Tur */}
                                    <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                                        <div className="flex justify-between items-start mb-3">
                                            <h3 className="font-semibold text-gray-900">Gastronomik Tur</h3>
                                            <span className="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full">
                                                Orta
                                            </span>
                                        </div>
                                        <p className="text-sm text-gray-600 mb-3">
                                            Yerel lezzetler ve restoranlar odaklı rota
                                        </p>
                                        <div className="flex items-center justify-between text-sm text-gray-500 mb-3">
                                            <div className="flex items-center">
                                                <i className="pi pi-clock mr-1"></i>
                                                <span>5-7 saat</span>
                                            </div>
                                            <div className="flex items-center">
                                                <i className="pi pi-map-marker mr-1"></i>
                                                <span>10 POI</span>
                                            </div>
                                        </div>
                                        <Button
                                            label="Rotayı Uygula"
                                            size="small"
                                            className="w-full bg-green-500 hover:bg-green-600 border-0 text-white"
                                            onClick={() => {
                                                toast.current.show({
                                                    severity: 'success',
                                                    summary: 'Rota Uygulandı',
                                                    detail: 'Gastronomik Tur uygulandı',
                                                    life: 3000
                                                });
                                            }}
                                        />
                                    </div>
                                </div>
                            </div>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default RouteBuilder;