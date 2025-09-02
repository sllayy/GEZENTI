import React, { useState, useEffect } from 'react';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5136';

const PastRoutesList = () => {
    const [routes, setRoutes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchPastRoutes = async () => {
            try {
                const token = localStorage.getItem('jwtToken');
                if (!token) {
                    throw new Error('Yetkilendirme tokenı bulunamadı.');
                }

                const response = await axios.get(
                    `${API_BASE_URL}/route/my-routes`,
                    {
                        headers: { Authorization: `Bearer ${token}` },
                    }
                );

                setRoutes(response.data);
            } catch (err) {
                console.error("Geçmiş rotalar çekilirken hata oluştu:", err);
                setError('Rotalar yüklenirken bir sorun oluştu.');
            } finally {
                setLoading(false);
            }
        };

        fetchPastRoutes();
    }, []);

    if (loading) {
        return <p className="text-gray-500">Geçmiş rotalarınız yükleniyor...</p>;
    }

    if (error) {
        return <p className="text-red-500">{error}</p>;
    }

    if (routes.length === 0) {
        return <p className="text-gray-500">Henüz oluşturulmuş bir geçmiş rotanız bulunmuyor.</p>;
    }

    return (
        <div className="space-y-4">
            {routes.map((route) => (
                <div
                    key={route.id}
                    className="border border-gray-200 p-4 rounded-lg shadow-sm hover:shadow-md transition-shadow"
                >
                    <h4 className="font-bold text-lg text-gray-800">
                        {route.startLocation} → {route.endLocation}
                    </h4>
                    <div className="flex space-x-4 text-gray-600 mt-2">
                        <span>Mesafe: {(route.distance / 1000).toFixed(2)} km</span>
                        <span>Süre: {Math.round(route.duration / 60)} dakika</span>
                    </div>
                    <p className="text-sm text-gray-400 mt-2">
                        Oluşturulma:{" "}
                        {new Date(route.createdAt).toLocaleDateString("tr-TR")}
                    </p>
                </div>
            ))}
        </div>
    );
};

export default PastRoutesList;
