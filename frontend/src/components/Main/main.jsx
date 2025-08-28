import React from "react";
import { FaCompass } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import MainCard from "./MainCard";



const Main = () => {
    const navigate = useNavigate();

    const destinations = [
        {
            image: "/kapadokya.jpg",
            title: "Kapadokya Balon Turu",
            location: "Nevşehir, Türkiye",
            description:
                "Büyüleyici Kapadokya manzarası eşliğinde unutulmaz bir balon deneyimi yaşayın.",
            tags: ["Balon", "Manzara", "Gündoğumu"],
            visitors: 1250,
            category: "Macera",
            rating: 4.8,
        },
        {
            image: "/pamukkale.jpg",
            title: "Pamukkale Travertenleri",
            location: "Denizli, Türkiye",
            description:
                "Doğanın muhteşem eseri beyaz travertenler ve antik Hierapolis şehri.",
            tags: ["Termal", "Antik Kent", "UNESCO"],
            visitors: 980,
            category: "Doğa",
            rating: 4.7,
        },
        {
            image: "/bogazici.jpg",
            title: "Boğaziçi Turu",
            location: "İstanbul, Türkiye",
            description:
                "İstanbul Boğazı'nın eşsiz güzelliğini tekne turuyla keşfedin.",
            tags: ["Tekne Turu", "Tarihi", "Boğaz"],
            visitors: 2150,
            category: "Kültür",
            rating: 4.6,
        },
    ];

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
                    Gezginlerimizin en çok tercih ettiği POI noktalarını keşfedin ve rotalarınıza ekleyin
                </p>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    {destinations.map((d, i) => (
                        <MainCard key={i} {...d} />
                    ))}
                </div>
            </div>
        </>
    );
};

export default Main;
