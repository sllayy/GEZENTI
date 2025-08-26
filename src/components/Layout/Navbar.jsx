import React from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Button } from 'primereact/button';
import { Avatar } from 'primereact/avatar';

// isLoggedIn ve setIsLoggedIn state'leri artık App.js'ten prop olarak geliyor.
const Navbar = ({ isLoggedIn, setIsLoggedIn }) => {
    const location = useLocation();
    const navigate = useNavigate();

    // Çıkış yapma fonksiyonu
    const handleLogout = () => {
        setIsLoggedIn(false); // Uygulama genelindeki giriş durumunu 'false' yap
        navigate('/');        // Kullanıcıyı ana sayfaya yönlendir
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
                                className={`text-sm font-medium transition-colors hover:text-blue-600 ${
                                    location.pathname === item.path
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
                                    label="T" // Gerçek projede kullanıcının baş harfi olabilir
                                    shape="circle" 
                                    size="large" 
                                    className="bg-blue-500 text-white" 
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
                                <Link to="/register"> {/* Kayıt ol sayfası için yönlendirme ekleyebilirsiniz */}
                                    <Button 
                                        label="Kayıt Ol" 
                                        className="bg-blue-400 text-black font-bold border-2 hover:bg-blue-500 rounded-lg p-button-sm px-4 py-2" 
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