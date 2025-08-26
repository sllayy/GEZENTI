import React, { useState } from 'react'; // useState'i import edin
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar';

// Sayfalar
import LoginPage from './pages/LoginPage'; // Oluşturduğunuz login sayfasını import edin
import RegisterPage from './pages/RegisterPage';

import RoutesPage from './pages/RoutesPage';
const AnaSayfa = () => <div className="p-8"><h1>Ana Sayfa</h1></div>;
const Rotalar = () => <div className="p-8"><h1>Rotalar Sayfası</h1></div>;
const Poi = () => <div className="p-8"><h1>POI Keşfet Sayfası</h1></div>;
const Harita = () => <div className="p-8"><h1>Harita Sayfası</h1></div>;
const RotaOlustur = () => <div className="p-8"><h1>Rota Oluşturma Sayfası</h1></div>;

function App() {
  // Giriş durumunu (state) App bileşeninde tutuyoruz
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
    <Router>
      <div>
        {/* Navbar'a state'i ve state'i değiştirecek fonksiyonu prop olarak geçiyoruz */}
        <Navbar isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

        <main>
          <Routes>
             <Route path="/routes" element={<RoutesPage />} />
            <Route path="/" element={<AnaSayfa />} />
            <Route path="/routes" element={<Rotalar />} />
            <Route path="/poi" element={<Poi />} />
            <Route path="/map" element={<Harita />} />
            <Route path="/route-builder" element={<RotaOlustur />} />
            
            {/* Login sayfası için yeni Route */}
            <Route 
              path="/login" 
              element={<LoginPage setIsLoggedIn={setIsLoggedIn} />} 
            />
            <Route path="/register" element={<RegisterPage setIsLoggedIn={setIsLoggedIn} />} />
          </Routes>

          
        </main>
      </div>
    </Router>
  );
}

export default App;