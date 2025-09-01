import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faPlus } from '@fortawesome/free-solid-svg-icons';
import { authAPI } from '../services/api';

const ProfilePage = () => {
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState('history');
    const [profileData, setProfileData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const token = localStorage.getItem("jwtToken");
        if (!token) {
            setError("Lütfen giriş yapın.");
            setLoading(false);
            return;
        }

        // Auth API'den kullanıcı bilgilerini al
        authAPI.getMe()
            .then(res => {
                setProfileData(res);
                setLoading(false);
            })
            .catch(err => {
                console.error("Profil verisi çekilirken hata:", err);
                if (err.response?.status === 401) {
                    setError("Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.");
                    localStorage.removeItem("jwtToken");
                } else {
                    setError("Profil bilgileri alınamadı. Lütfen tekrar deneyin.");
                }
                setLoading(false);
            });
    }, []);

    const handleCreateRoute = () => {
        navigate("/route-builder");
    };

    if (loading) return <div>Profil bilgileri yükleniyor...</div>;
    if (error) return <div>Hata: {error}</div>;

    const userName = profileData?.firstName || "User";

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="flex flex-col md:flex-row justify-between items-center p-6 bg-white rounded-xl shadow-md mb-6 space-y-4 md:space-y-0">

                {/* Sol taraf: Avatar ve kullanıcı adı */}
                <div className="flex items-center space-x-4">
                    <div className="w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center text-gray-700 text-4xl">
                        {userName.charAt(0).toUpperCase()}
                    </div>
                    <div>
                        <h2 className="text-2xl font-bold">{userName}</h2>
                        <p className="text-gray-500">Seyahat tutkunu ve rota yaratıcısı</p>
                        <p className="text-gray-600">{profileData?.email}</p>
                    </div>
                </div>

                {/* Sağ taraf: Butonlar */}
                <div className="flex space-x-4">
                    <button className="flex items-center px-5 py-3 border border-gray-300 text-gray-700 rounded-lg shadow hover:bg-gray-100 transition duration-300 ease-in-out hover:scale-105">
                        <FontAwesomeIcon icon={faEdit} className="mr-2" />
                        Profili Düzenle
                    </button>

                    <button
                        onClick={handleCreateRoute}
                        className="flex items-center px-5 py-3 bg-blue-500 text-white rounded-lg shadow-lg hover:bg-blue-600 transition duration-300 ease-in-out hover:scale-105"
                    >
                        <FontAwesomeIcon icon={faPlus} className="mr-2" />
                        Rota Oluştur
                    </button>
                </div>
            </div>

            {/* Tab Navigation */}
            <div className="bg-white rounded-xl shadow-md p-6">
                {/* Tab Başlıkları */}
                <div className="flex border-b border-gray-200 mb-4">
                    <button
                        className={`flex-1 py-2 text-center font-semibold transition-colors duration-300 ${activeTab === 'history'
                            ? 'border-b-4 border-blue-500 text-blue-500'
                            : 'text-gray-500 hover:text-blue-500'
                            }`}
                        onClick={() => setActiveTab('history')}
                    >
                        Geçmiş Rotalarım
                    </button>

                    <button
                        className={`flex-1 py-2 text-center font-semibold transition-colors duration-300 ${activeTab === 'favorites'
                            ? 'border-b-4 border-blue-500 text-blue-500'
                            : 'text-gray-500 hover:text-blue-500'
                            }`}
                        onClick={() => setActiveTab('favorites')}
                    >
                        Favorilerim
                    </button>
                </div>

                {/* Tab İçerikleri */}
                <div>
                    {activeTab === 'history' && (
                        <div>
                            <p>Buraya geçmiş rotaların listesi gelecek...</p>
                        </div>
                    )}

                    {activeTab === 'favorites' && (
                        <div>
                            <p>Buraya favori rotaların listesi gelecek...</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ProfilePage;
