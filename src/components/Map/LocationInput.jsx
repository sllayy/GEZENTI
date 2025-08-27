// src/components/Map/LocationInput.jsx

import React, { useState, useEffect, useRef } from 'react';
import { FaMapMarkerAlt, FaFlag, FaSearch, FaUser } from 'react-icons/fa';

const LocationInput = ({ placeholder, onSelect, value, type }) => {
    const [searchTerm, setSearchTerm] = useState(value || '');
    const [suggestions, setSuggestions] = useState([]);
    const [isOpen, setIsOpen] = useState(false);
    const inputRef = useRef(null);

    useEffect(() => {
        setSearchTerm(value || '');
    }, [value]);

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (inputRef.current && !inputRef.current.contains(event.target)) {
                setIsOpen(false);
                setSuggestions([]);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    const handleSearch = async (term) => {
        if (term.length < 3) {
            setSuggestions([]);
            return;
        }
        try {
            const response = await fetch(`https://nominatim.openstreetmap.org/search?q=${term}&format=json&limit=5&countrycodes=tr`);
            const data = await response.json();
            const newSuggestions = data.map(item => ({
                name: item.display_name,
                coords: [parseFloat(item.lon), parseFloat(item.lat)],
            }));
            setSuggestions(newSuggestions);
        } catch (error) {
            console.error('Arama hatası:', error);
        }
    };

    const handleGeolocation = () => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    const coords = [position.coords.longitude, position.coords.latitude];
                    const location = { name: 'Mevcut Konumum', coords };
                    onSelect(location);
                    setIsOpen(false);
                    setSearchTerm(location.name);
                    alert('Konumunuz belirlendi.');
                },
                () => {
                    alert('Konumunuza erişilemedi. Lütfen tarayıcı ayarlarınızı kontrol edin.');
                }
            );
        } else {
            alert('Tarayıcınız konum hizmetlerini desteklemiyor.');
        }
    };

    const handleSelectSuggestion = (suggestion) => {
        onSelect(suggestion);
        setSearchTerm(suggestion.name);
        setIsOpen(false);
        setSuggestions([]);
    };

    return (
        <div className="relative" ref={inputRef}>
            <button
                className="w-full text-left px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 flex items-center gap-2 bg-white text-gray-700"
                onClick={() => setIsOpen(!isOpen)}
            >
                {type === 'start' ? <FaMapMarkerAlt className="text-red-500" /> : <FaFlag className="text-green-500" />}
                <span>{value || placeholder}</span>
            </button>

            {isOpen && (
                <div className="absolute z-20 w-full bg-white border border-gray-300 rounded-lg shadow-lg mt-1 overflow-hidden animate-slide-down">
                    <div className="p-2 border-b">
                        <div className="relative flex items-center">
                            <FaSearch className="absolute left-3 text-gray-400" />
                            <input
                                type="text"
                                placeholder="Bir yer arayın..."
                                className="w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none focus:ring-1 focus:ring-blue-500"
                                value={searchTerm}
                                onChange={(e) => {
                                    setSearchTerm(e.target.value);
                                    handleSearch(e.target.value);
                                }}
                            />
                        </div>
                        <button
                            onClick={handleGeolocation}
                            className="w-full mt-2 flex items-center justify-center gap-2 px-4 py-2 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition duration-300"
                        >
                            <FaUser />
                            Mevcut Konumumu Kullan
                        </button>
                    </div>
                    {suggestions.length > 0 && (
                        <ul className="max-h-60 overflow-y-auto">
                            {suggestions.map((suggestion, index) => (
                                <li
                                    key={index}
                                    className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2"
                                    onClick={() => handleSelectSuggestion(suggestion)}
                                >
                                    <FaMapMarkerAlt className="text-gray-400" />
                                    <span>{suggestion.name}</span>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            )}
        </div>
    );
};

export default LocationInput;
