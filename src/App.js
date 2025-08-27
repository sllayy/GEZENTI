import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar';
import Footer from './components/Layout/Footer'; // Remote'dan eklenen Footer bileşeni
import Main from './components/Main/main'; // Remote'dan eklenen Main bileşeni

// Sayfalar
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import RoutesPage from './pages/RoutesPage';
import DiscoverPoi from './components/Discover_poi/DiscoverPoi'; // Remote'dan eklenen POI bileşeni

// Bileşen tanımlamalarını birleştirelim
const AnaSayfa = () => <div className="p-8"><h1>Ana Sayfa</h1></div>;
const Rotalar = () => <div className="p-8"><h1>Rotalar Sayfası</h1></div>;
const Harita = () => <div className="p-8"><h1>Harita Sayfası</h1></div>;
const RotaOlustur = () => <div className="p-8"><h1>Rota Oluşturma Sayfası</h1></div>;

function App() {
  // Giriş durumunu (state) App bileşeninde tutuyoruz
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
    <Router>
      <div className="flex flex-col min-h-screen">
        {/* Navbar'a state'i ve state'i değiştirecek fonksiyonu prop olarak geçiyoruz */}
        <Navbar isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

        {/* Sayfa içeriği */}
        <main className="flex-grow">
          <Routes>
            {/* Ana sayfa için hem sizin hem de remote'daki kodu birleştirelim */}
            <Route path="/" element={<Main />} />
            
            {/* Sizin Rotalar sayfası bileşeninizi kullanalım */}
            <Route path="/routes" element={<RoutesPage />} />
            
            {/* Remote'dan gelen POI bileşenini kullanalım */}
            <Route path="/poi" element={<DiscoverPoi />} />
            
            <Route path="/map" element={<Harita />} />
            <Route path="/route-builder" element={<RotaOlustur />} />
            
            {/* Login ve Register sayfaları için sizin rotalarınızı kullanalım */}
            <Route 
              path="/login" 
              element={<LoginPage setIsLoggedIn={setIsLoggedIn} />} 
            />
            <Route path="/register" element={<RegisterPage setIsLoggedIn={setIsLoggedIn} />} />
          </Routes>
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
