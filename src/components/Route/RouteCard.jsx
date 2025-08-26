import React from 'react';
import { Avatar } from 'primereact/avatar';

const RouteCard = ({ route }) => {
    const getDifficultyClass = (difficulty) => {
        switch (difficulty.toLowerCase()) {
            case 'kolay': return 'bg-green-500 text-white';
            case 'orta': return 'bg-yellow-500 text-white';
            case 'zor': return 'bg-red-500 text-white';
            default: return 'bg-gray-500 text-white';
        }
    };

    return (
        <div className="bg-white rounded-2xl shadow-lg overflow-hidden transition-transform transform hover:-translate-y-1 hover:shadow-xl">
            <div className="relative">
                <img src={route.imageUrl} alt={route.title} className="w-full h-48 object-cover" />
                <div className="absolute top-3 left-3 flex flex-col space-y-2">
                    <span className={`px-3 py-1 text-xs font-semibold rounded-full ${getDifficultyClass(route.difficulty)}`}>
                        {route.difficulty}
                    </span>
                </div>
                <div className="absolute top-3 right-3">
                    <span className="px-3 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800 flex items-center">
                        <i className="pi pi-globe mr-1" style={{ fontSize: '0.75rem' }}></i>
                        Herkese Açık
                    </span>
                </div>
            </div>

            <div className="p-5">
                <h3 className="text-xl font-bold text-gray-800">{route.title}</h3>
                <p className="text-gray-600 text-sm mt-1 mb-4">{route.description}</p>

                <div className="flex items-center space-x-4 text-sm text-gray-700 mb-4">
                    <div className="flex items-center"><i className="pi pi-clock mr-1.5"></i>{route.duration}</div>
                    <div className="flex items-center"><i className="pi pi-map-marker mr-1.5"></i>{route.poiCount} POI</div>
                    <div className="flex items-center"><i className="pi pi-star-fill text-yellow-400 mr-1.5"></i>{route.rating}</div>
                </div>

                <div className="flex flex-wrap gap-2 mb-4">
                    {route.tags.map(tag => (
                        <span key={tag} className="px-3 py-1 text-xs font-medium rounded-full bg-gray-100 text-gray-700">{tag}</span>
                    ))}
                </div>

                <div className="border-t border-gray-200 pt-4 flex justify-between items-center">
                    <div className="flex items-center">
                        <Avatar label={route.author.name.charAt(0)} size="normal" shape="circle" className="mr-3 bg-gray-200" />
                        <span className="text-sm font-medium text-gray-800">{route.author.name}</span>
                    </div>
                    <button className="px-4 py-2 font-semibold text-sm rounded-lg text-white bg-gradient-to-r from-green-400 to-teal-500 hover:from-green-500 hover:to-teal-600">
                        İncele
                    </button>
                </div>
            </div>
        </div>
    );
};

export default RouteCard;