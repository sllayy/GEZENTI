import React, { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { Button } from 'primereact/button';
import { Avatar } from 'primereact/avatar';

const Navbar = () => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const location = useLocation();

    const navItems = [
        { label: 'Ana Sayfa', path: '/' },
        { label: 'Rotalar', path: '/routes' },
        { label: 'POI Keşfet', path: '/poi' },
        { label: 'Harita', path: '/map' },
        { label: 'Rota Oluştur', path: '/route-builder' }
    ];

    return (
        <nav className="bg-gray-100 border-b border-gray-300 sticky top-0 z-50">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between items-center h-16">
                    {/* Logo */}
                    <Link to="/" className="flex items-center space-x-2">
                        <img src="/LOGO2.png" alt="Gezenti Logo" className="h-10 w-10" />
                        <span className="text-xl font-bold text-blue-600">
                            Gezenti
                        </span>
                    </Link>

                    {/* Navigation Links */}
                    <div className="hidden md:flex space-x-8">
                        {navItems.map((item) => (
                            <Link
                                key={item.path}
                                to={item.path}
                                className={`text-sm font-medium transition-colors hover:text-blue-600 ${location.pathname === item.path
                                    ? 'text-blue-600 border-b-2 border-blue-600'
                                    : 'text-gray-500'
                                    }`}
                            >
                                {item.label}
                            </Link>
                        ))}
                    </div>

                    {/* Auth Section */}
                    <div className="flex items-center space-x-4">
                        {isLoggedIn ? (
                            <div className="flex items-center space-x-3">
                                <Button label="Bildirimler" className="p-button-text p-button-sm" />
                                <Avatar image="/placeholder-user.jpg" shape="circle" size="large" />
                            </div>
                        ) : (
                            <div className="flex items-center space-x-5">
                                <Button label="Giriş Yap" className="p-button-text p-button-sm font-bold" />
                                <Button label="Kayıt Ol" className="bg-blue-400 text-black font-bold  border-2  hover:bg-blue-500 rounded-lg p-button-sm px-4 py-2" />
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
