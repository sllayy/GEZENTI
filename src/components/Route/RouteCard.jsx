import React from 'react';

const RouteCard = ({ route }) => {


    return (
        <div className="bg-white rounded-2xl shadow-md overflow-hidden border border-gray-200 hover:shadow-xl transition-shadow">
            <div className="relative">
                <div className="w-full h-48 bg-gray-200 flex items-center justify-center">
                    <span className="text-gray-400">{route.title}</span>
                </div>


            </div>
            <div className="p-5">
                <h3 className="text-2xl font-bold text-gray-800 mb-3">{route.title}</h3>
                <div className="flex items-center space-x-4 text-sm text-gray-600 mb-4">
                    <span>POI</span>
                    <span className="font-semibold">{route.rating}</span>
                </div>
                <div className="flex flex-wrap gap-2 mb-5">
                    {(route.tags || []).map(tag => (
                        <span key={tag} className="px-4 py-1.5 text-sm rounded-lg bg-gray-100 text-gray-700">{tag}</span>
                    ))}
                </div>
                <div className="border-t border-gray-200 my-4"></div>
                <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-gray-800">{route.author?.name}</span>
                    <button className="px-5 py-2 font-semibold text-sm rounded-lg text-white bg-orange-500 hover:bg-orange-600">
                        İncele
                    </button>
                </div>
                <div className="flex items-center text-sm text-gray-500 mt-3">
                    <span>{route.likes}</span>
                    <span className="mx-2">•</span>
                    <span>{route.comments} yorum</span>
                </div>
            </div>
        </div>
    );
};

export default RouteCard;