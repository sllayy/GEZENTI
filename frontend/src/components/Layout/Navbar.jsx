import React from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Button } from 'primereact/button';
import { Avatar } from 'primereact/avatar';

// isLoggedIn, setIsLoggedIn ve userName state'leri App.js'ten prop olarak geliyor.
const Navbar = ({ isLoggedIn, setIsLoggedIn, userName, avatarIndex }) => {
    const location = useLocation();
    const navigate = useNavigate();

    const avatars = [
        "/assets/avatar1.png",
        "/assets/avatar2.png",
        "/assets/avatar3.png",
        "/assets/avatar4.png",
        "/assets/avatar5.png",
        "/assets/avatar6.png",
        "/assets/avatar7.png"
    ];

    // Çıkış yapma fonksiyonu
    const handleLogout = () => {
        // Local storage temizle
        localStorage.removeItem("jwtToken");
        localStorage.removeItem("userName");
        localStorage.removeItem("avatarIndex");

        // State güncelle
        setIsLoggedIn(false);

        // Ana sayfaya yönlendir
        navigate('/');
    };

    // Navigasyon linklerinin listesi
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
                    {/* Logo Alanı */}
                    <Link to="/" className="flex items-center space-x-2">
                        <img src="/LOGO2.png" alt="Gezenti Logo" className="h-10 w-10" />
                        <span className="text-xl font-bold text-blue-600">
                            Gezenti
                        </span>
                    </Link>

                    {/* Navigasyon Linkleri */}
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

                    {/* Giriş/Kullanıcı Alanı */}
                    <div className="flex items-center space-x-4">
                        {isLoggedIn ? (
                            // Kullanıcı giriş yapmışsa bu bölüm gösterilir
                            <div className="flex items-center space-x-3">
                                <Avatar
                                    image={avatars[avatarIndex]}
                                    shape="circle"
                                    size="large"
                                    className="cursor-pointer"
                                    onClick={() => navigate("/profile")}
                                />
                                <Button
                                    label="Çıkış Yap"
                                    icon="pi pi-sign-out"
                                    className="p-button-text p-button-sm font-bold text-red-500"
                                    onClick={handleLogout}
                                />
                            </div>
                        ) : (
                            // Kullanıcı giriş yapmamışsa bu bölüm gösterilir
                            <div className="flex items-center space-x-5">
                                <Link to="/login">
                                    <Button
                                        label="Giriş Yap"
                                        className="p-button-text p-button-sm font-bold"
                                    />
                                </Link>
                                <Link to="/register">
                                    <Button
                                        label="Kayıt Ol"
                                        className="bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-transform transform hover:scale-105 text-black font-bold border-2 rounded-lg p-button-sm px-4 py-2"
                                    />
                                </Link>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
