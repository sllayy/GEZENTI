import React, { useState } from 'react';
import FilterSidebar from '../components/Route/FilterSidebar';
import RouteCard from '../components/Route/RouteCard';

const mockRoutes = [
    {
        id: 1, title: 'İstanbul Tarihi Yarımada Turu', description: 'Sultanahmet, Ayasofya, Topkapı Sarayı ve Grand Bazaar\'ı içeren 2 günlük İstanbul turu',
        imageUrl: 'https://images.unsplash.com/photo-1541561085361-3454c67b9b1e?ixlib=rb-4.0.3&q=85&fm=jpg&crop=entropy&cs=srgb&w=1600',
        difficulty: 'Kolay', duration: '2 gün', poiCount: 12, rating: 4.8,
        tags: ['Tarih', 'Kültür', 'Yürüyüş'],
        author: { name: 'Ahmet Yılmaz' }
    },
    {
        id: 2, title: 'Kapadokya Macera Rotası', description: 'Balon turu, yeraltı şehirleri, trekking ve çanak kaya formasyonları',
        imageUrl: 'https://images.unsplash.com/photo-1579882246019-3919d3f55e46?ixlib=rb-4.0.3&q=85&fm=jpg&crop=entropy&cs=srgb&w=1600',
        difficulty: 'Orta', duration: '3 gün', poiCount: 8, rating: 4.9,
        tags: ['Doğa', 'Macera', 'Fotoğraf'],
        author: { name: 'Elif Kaya' }
    },
];

const RoutesPage = () => {
    const [activeTab, setActiveTab] = useState('Tümü');
    const tabs = ['Tümü', 'Popüler', 'Yeni', 'En Çok Beğenilen'];

    return (
        <div className="bg-gray-50 min-h-screen">
            <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
                
                <div className="flex justify-between items-center mb-6">
                    <div>
                        <h1 className="text-4xl font-bold text-gray-900">Rotalar</h1>
                        <p className="mt-1 text-lg text-gray-600">Kullanıcılar tarafından oluşturulan rotaları keşfedin ve kendi rotalarınızı paylaşın</p>
                    </div>
                    <button className="hidden sm:flex items-center px-5 py-3 font-semibold text-white rounded-xl bg-gradient-to-r from-green-400 to-orange-400 hover:from-green-500 hover:to-orange-500 shadow-md">
                        <i className="pi pi-plus mr-2 font-bold"></i>
                        Yeni Rota Oluştur
                    </button>
                </div>

                <div className="bg-white p-2 rounded-2xl shadow-sm flex justify-between items-center mb-8">
                    <div className="flex space-x-1">
                        {tabs.map(tab => (
                            <button key={tab} onClick={() => setActiveTab(tab)}
                                className={`px-4 py-2 text-sm font-semibold rounded-lg transition-colors ${
                                    activeTab === tab ? 'bg-green-100 text-green-800' : 'text-gray-600 hover:bg-gray-100'
                                }`}>
                                {tab}
                            </button>
                        ))}
                    </div>
                    <div className="flex space-x-2">
                        <button className="px-4 py-2 text-sm font-semibold text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-100 flex items-center">
                            <i className="pi pi-filter mr-2"></i>Filtrele
                        </button>
                        <button className="px-4 py-2 text-sm font-semibold text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-100 flex items-center">
                            <i className="pi pi-sort-alt mr-2"></i>Sırala
                        </button>
                    </div>
                </div>

                <main className="grid grid-cols-1 lg:grid-cols-4 gap-8">
                    <FilterSidebar />
                    <div className="lg:col-span-3 grid grid-cols-1 md:grid-cols-2 gap-6">
                        {mockRoutes.map(route => (
                            <RouteCard key={route.id} route={route} />
                        ))}
                    </div>
                </main>

            </div>
        </div>
    );
};

export default RoutesPage;