import React, { useState, useEffect } from "react";
// `react-icons/fa` ve `./MainCard` import yolları düzeltildi.
import { FaCompass } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import MainCard from "./MainCard";

// API çağrılarını yönetecek bir servis fonksiyonu oluşturalım
const getPois = async (sortBy = 'name') => {
    try {
        const response = await fetch(`https://localhost:7248/api/pois?sortBy=${sortBy}`); 
        if (!response.ok) {
            throw new Error("Veri çekme işlemi başarısız oldu.");
        }
        return await response.json();
    } catch (error) {
        console.error("API'den veri çekilirken bir hata oluştu:", error);
        throw error;
    }
};

const Main = () => {
    const navigate = useNavigate();

    // API'den gelecek olan veriyi tutmak için state oluşturuyoruz.
    const [destinations, setDestinations] = useState([]);

    // Yükleme (loading) ve hata (error) durumlarını yönetmek için ek state'ler.
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    // Bileşen ilk yüklendiğinde API'ye istek göndermek için useEffect kullanıyoruz.
    useEffect(() => {
        const fetchPois = async () => {
            try {
                // Popüler destinasyonlar için 'populer' parametresini gönderiyoruz.
                const data = await getPois("populer"); 
                setDestinations(data); 
            } catch (err) {
                setError(err.message); // Hata durumunu kaydet.
            } finally {
                setIsLoading(false); // Yükleme durumunu bitir.
            }
        };

        fetchPois();
    }, []); // Boş bağımlılık dizisi, bu kodun sadece bir kere çalışmasını sağlar.

    return (
        <>
            {/* Hero Alanı */}
            <div className="relative w-full mb-20" style={{ height: "500px" }}>
                <img
                    src="/anasayfa.png"
                    alt="Ana Sayfa"
                    className="w-full h-full object-cover rounded-lg"
                />
                <div className="absolute inset-0 flex flex-col items-center justify-center text-white bg-black bg-opacity-30 rounded-lg p-6">
                    <h1 className="text-4xl md:text-5xl font-bold mb-4 text-center">
                        Hayalinizdeki
                        <span className="text-orange-400"> Rotayı </span>
                        Keşfedin ve Yaratın
                    </h1>
                    <p className="text-lg md:text-2xl text-center max-w-3xl">
                        Binlerce <span className="text-orange-400">POI</span> noktasından
                        rotanızı oluşturun, diğer gezginlerle paylaşın ve unutulmaz maceralar
                        yaşayın.
                    </p>
                    <button
                        onClick={() => navigate("/poi")}
                        className="mt-12 flex items-center space-x-2 bg-orange-500 hover:bg-orange-600 px-6 py-3 rounded-lg font-semibold text-white shadow-lg transition"
                    >
                        <FaCompass className="text-xl" />
                        <span>Rotaları Keşfet</span>
                    </button>
                </div>
            </div>

            {/* Kart Grid Alanı */}
            <div className="max-w-7xl mx-auto px-4 py-10">
                <h2 className="text-3xl font-bold text-center mb-6">Popüler Destinasyonlar</h2>
                <p className="text-lg md:text-2xl text-center max-w-3xl mx-auto text-gray-600 mb-8">
                    Gezginlerimizin en çok tercih ettiği POI noktalarını keşfedin ve rotalarına ekleyin
                </p>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    {/* Yükleme, hata veya veri olmadığı durumlar için koşullu renderlama */}
                    {isLoading && <p>Veriler yükleniyor...</p>}
                    {error && <p className="text-red-500">Hata: {error}</p>}
                    {!isLoading && !error && destinations.length > 0 ? (
                        // Veri varsa, gelen verileri haritala ve MainCard bileşenine gönder.
                        destinations.map((d) => <MainCard key={d.id} {...d} />)
                    ) : (
                        // Veri yoksa veya bir hata oluştuysa kullanıcıya bilgi ver.
                        !isLoading && !error && <p>Gösterilecek destinasyon bulunamadı.</p>
                    )}
                </div>
            </div>
        </>
    );
};

export default Main;
