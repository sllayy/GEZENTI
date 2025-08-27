import React from "react";
import { FaStar, FaUsers, FaMapMarkerAlt } from "react-icons/fa";

const MainCard = ({
    image,
    title,
    location,
    description,
    tags,
    visitors,
    category,
    rating,
}) => {
    return (
        <div className="bg-white rounded-lg shadow-md overflow-hidden relative mb-20">
            {/* Üst görsel */}
            <img src={image} alt={title} className="w-full h-48 object-cover" />

            {/* Rating ve kategori */}
            <div className="absolute top-2 left-2 bg-gray-900 text-white text-xs px-2 py-1 rounded flex items-center space-x-1">
                <FaStar className="text-yellow-400" />
                <span>{rating}</span>
            </div>
            <div className="absolute top-2 right-2 bg-gray-200 text-gray-700 text-xs px-2 py-1 rounded">
                {category}
            </div>

            {/* İçerik */}
            <div className="p-4">
                <h3 className="font-bold text-lg">{title}</h3>
                <p className="text-sm text-gray-500 flex items-center">
                    <FaMapMarkerAlt className="mr-1" /> {location}
                </p>
                <p className="text-sm text-gray-600 mt-2">{description}</p>

                {/* Etiketler */}
                <div className="flex flex-wrap gap-2 mt-3">
                    {tags.map((tag, index) => (
                        <span
                            key={index}
                            className="bg-gray-100 text-gray-700 px-2 py-1 text-xs rounded-full"
                        >
                            {tag}
                        </span>
                    ))}
                </div>

                {/* Alt kısım */}
                <div className="flex justify-between items-center mt-4 text-sm text-gray-500">
                    <div className="flex items-center space-x-1">
                        <FaUsers />
                        <span>{visitors} ziyaretçi</span>
                    </div>
                    <div className="flex space-x-2">
                        <button className="px-3 py-1 border rounded-lg hover:bg-gray-100">
                            Detaylar
                        </button>
                        <button className="px-3 py-1 bg-orange-400 text-white rounded-lg hover:bg-orange-500">
                            Rotaya Ekle
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default MainCard;
