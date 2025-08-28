import React, { useState } from "react";
import { FaStar, FaUsers, FaMapMarkerAlt, FaSearch, FaFilter } from "react-icons/fa";
import MainCard from "../components/Main/MainCard";

const DiscoverPoi = () => {
    // Örnek veriler - Kendi verilerinizle değiştirebilirsiniz
    const [allPois] = useState([
        {
            id: 1,
            image: "",
            title: "Kapadokya Balon Turu",
            location: "Nevşehir, Türkiye",
            description: "Peri bacaları üzerinde unutulmaz bir balon deneyimi.",
            tags: ["Doğa", "Macera", "Fotoğrafçılık"],
            visitors: "250K+",
            category: "Macera",
            rating: "4.8",
        },
        {
            id: 2,
            image: "",
            title: "Pamukkale Travertenleri",
            location: "Denizli, Türkiye",
            description: "Beyaz traverten terasları ve Hierapolis antik kenti.",
            tags: ["Doğa", "Tarih", "Termal"],
            visitors: "300K+",
            category: "Doğa",
            rating: "4.7",
        },
        {
            id: 3,
            image: "",
            title: "Boğaziçi Turu",
            location: "İstanbul, Türkiye",
            description: "Boğazın eşsiz güzelliklerini denizden keşfedin.",
            tags: ["Kültür", "Deniz", "Tarihi Mekan"],
            visitors: "500K+",
            category: "Kültür",
            rating: "4.6",
        },
        {
            id: 4,
            image: "",
            title: "Efes Antik Kenti",
            location: "İzmir, Türkiye",
            description: "Antik Roma ve Yunan döneminin büyüleyici kalıntıları.",
            tags: ["Tarih", "Arkeoloji", "Kültür"],
            visitors: "400K+",
            category: "Tarih",
            rating: "4.9",
        },
        {
            id: 5,
            image: "",
            title: "Nemrut Dağı",
            location: "Adıyaman, Türkiye",
            description: "Tanrı heykelleri ile güneşin batışını izleyin.",
            tags: ["Tarih", "Doğa", "Manzara"],
            visitors: "150K+",
            category: "Tarih",
            rating: "4.7",
        },
        {
            id: 6,
            image: "",
            title: "Ölüdeniz Plajı",
            location: "Fethiye, Türkiye",
            description: "Turkuaz renkli denizi ve yamaç paraşütü aktivitesi.",
            tags: ["Deniz", "Doğa", "Macera"],
            visitors: "600K+",
            category: "Doğa",
            rating: "4.9",
        },
        {
            id: 7,
            image: "",
            title: "Pamukkale Travertenleri",
            location: "Denizli, Türkiye",
            description: "Beyaz traverten terasları ve Hierapolis antik kenti.",
            tags: ["Doğa", "Tarih", "Termal"],
            visitors: "300K+",
            category: "Doğa",
            rating: "4.7",
        },
        {
            id: 8,
            image: "",
            title: "Boğaziçi Turu",
            location: "İstanbul, Türkiye",
            description: "Boğazın eşsiz güzelliklerini denizden keşfedin.",
            tags: ["Kültür", "Deniz", "Tarihi Mekan"],
            visitors: "500K+",
            category: "Kültür",
            rating: "4.6",
        },
        {
            id: 9,
            image: "",
            title: "Efes Antik Kenti",
            location: "İzmir, Türkiye",
            description: "Antik Roma ve Yunan döneminin büyüleyici kalıntıları.",
            tags: ["Tarih", "Arkeoloji", "Kültür"],
            visitors: "400K+",
            category: "Tarih",
            rating: "4.9",
        },
    ]);
    const [visiblePois, setVisiblePois] = useState(6); // Başlangıçta 6 kart göster

    const handleLoadMore = () => {
        setVisiblePois((prevVisiblePois) => prevVisiblePois + 3); // Her tıklamada 3 kart daha yükle
    };

    return (



        <main className="container mx-auto px-4 py-8">
            {/* Başlık ve Açıklama */}
            <section className="text-center mb-8">
                <h1 className="text-4xl font-bold text-gray-800 mb-2">POI Keşfet</h1>
                <p className="text-gray-600 text-lg">
                    İlgi çekici noktaları keşfedin ve rotalarınıza ekleyin
                </p>
            </section>

            {/* Arama ve Kategori Filtreleri */}
            <section className="bg-white p-6 rounded-lg shadow-md mb-8">
                <div className="flex flex-col md:flex-row items-center justify-between space-y-4 md:space-y-0 md:space-x-4">
                    <div className="relative flex-grow w-full md:w-auto">
                        <FaSearch className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                        <input
                            type="text"
                            placeholder="POI ara... (şehir, kategori, isim)"
                            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-400"
                        />
                    </div>
                    <div className="flex flex-wrap justify-center gap-2">
                        {["Tümü", "Doğa", "Kültür", "Macera", "Tarih", "Gastronomi"].map(
                            (category) => (
                                <button
                                    key={category}
                                    className={`px-4 py-2 rounded-lg text-sm ${category === "Tümü"
                                        ? "bg-orange-500 text-white"
                                        : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                                        }`}
                                >
                                    {category}
                                </button>
                            )
                        )}
                    </div>
                </div>
            </section>

            {/* POI Sayısı ve Filtre Butonu */}
            <section className="flex justify-between items-center mb-6">
                <p className="text-gray-700 font-medium">
                    <span className="font-bold">{allPois.length}</span> POI bulundu
                </p>
            </section>

            {/* Kartlar */}
            <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {allPois.slice(0, visiblePois).map((poi) => (
                    <MainCard
                        key={poi.id}
                        image={poi.image}
                        title={poi.title}
                        location={poi.location}
                        description={poi.description}
                        tags={poi.tags}
                        visitors={poi.visitors}
                        category={poi.category}
                        rating={poi.rating}
                    />
                ))}
            </section>

            {/* Daha Fazla Yükle Butonu */}
            {visiblePois < allPois.length && (
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