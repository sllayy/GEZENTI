import React, { useState, useEffect } from "react";
import { FaSearch } from "react-icons/fa";
import MainCard from "../components/Main/MainCard";

const DiscoverPoi = () => {
    // State'ler
    const [pois, setPois] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [visiblePois, setVisiblePois] = useState(6);
    const [searchTerm, setSearchTerm] = useState("");
    const [selectedCategory, setSelectedCategory] = useState("Tümü");

    // Backend'deki PoiCategory enum'una göre kategori listesi
    const categories = [
        "Tümü",
        "Yemek",
        "Tarih",
        "Sahil",
        "Kültür",
        "Alışveriş",
        "Park",
        "Müze",
        "Restoran",
        "Genel",
        "Sanat",
        "Doğa",
        "Eğlence",
        "Müzik",
        "Karayolu",
        "KültürelTesisler",
        "OnemliNoktalar",
        "TarihiTuristikTesisler",
    ];

    // API'den verileri çeken useEffect hook'u
    useEffect(() => {
        const fetchPois = async () => {
            setLoading(true);
            setError(null);

            let url = `https://localhost:7248/api/Pois?`;
            
            if (searchTerm) {
                url += `searchTerm=${searchTerm}&`;
            }
            
            // Seçili kategori "Tümü" değilse ve kategori listesinde varsa
            if (selectedCategory !== "Tümü" && categories.includes(selectedCategory)) {
                url += `category=${selectedCategory}&`;
            }
            
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error("Veri alınamadı, sunucu hatası.");
                }
                const data = await response.json();
                setPois(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchPois();
    }, [searchTerm, selectedCategory]);

    const handleLoadMore = () => {
        setVisiblePois((prevVisiblePois) => prevVisiblePois + 3);
    };

    return (
        <main className="container mx-auto px-4 py-8">
            <section className="text-center mb-8">
                <h1 className="text-4xl font-bold text-gray-800 mb-2">POI Keşfet</h1>
                <p className="text-gray-600 text-lg">
                    İlgi çekici noktaları keşfedin ve rotalarınıza ekleyin
                </p>
            </section>

            <section className="bg-white p-6 rounded-lg shadow-md mb-8">
                <div className="flex flex-col md:flex-row items-center justify-between space-y-4 md:space-y-0 md:space-x-4">
                    <div className="relative flex-grow w-full md:w-auto">
                        <FaSearch className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                        <input
                            type="text"
                            placeholder="POI ara... (şehir, kategori, isim)"
                            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-400"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    <div className="flex flex-wrap justify-center gap-2">
                        {categories.map(
                            (category) => (
                                <button
                                    key={category}
                                    className={`px-4 py-2 rounded-lg text-sm ${selectedCategory === category
                                        ? "bg-orange-500 text-white"
                                        : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                                        }`}
                                    onClick={() => setSelectedCategory(category)}
                                >
                                    {category}
                                </button>
                            )
                        )}
                    </div>
                </div>
            </section>

            {loading && <div className="text-center text-xl text-gray-500 mt-10">POI'ler yükleniyor...</div>}
            {error && <div className="text-center text-xl text-red-500 mt-10">Hata: {error}</div>}

            {!loading && !error && (
                <>
                    <section className="flex justify-between items-center mb-6">
                        <p className="text-gray-700 font-medium">
                            <span className="font-bold">{pois.length}</span> POI bulundu
                        </p>
                    </section>

                    <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                        {pois.slice(0, visiblePois).map((poi) => (
                            <MainCard
                                key={poi.id}
                                image={poi.imageUrl}
                                name={poi.name || poi.description} 
                                location={poi.description}
                                tags={poi.tags} 
                                visitors={poi.visitors}
                                category={poi.category}
                                rating={poi.rating}
                            />
                        ))}
                    </section>
                </>
            )}

            {visiblePois < pois.length && (
                <div className="flex justify-center mt-12 mb-8">
                    <button
                        onClick={handleLoadMore}
                        className="bg-orange-500 text-white px-6 py-3 rounded-lg text-lg font-semibold hover:bg-orange-600 transition duration-300"
                    >
                        + Daha Fazla Yükle
                    </button>
                </div>
            )}
        </main>
    );
};

export default DiscoverPoi;