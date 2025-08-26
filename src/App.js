// src/App.js

import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar'; // Navbar bileşenini doğru yoldan import edin

// Sayfalarınızın geleceği yer (Örnek olarak AnaSayfa bileşeni)
const AnaSayfa = () => <div className="p-8"><h1></h1></div>;
const Rotalar = () => <div className="p-8"><h1>Rotalar Sayfası</h1></div>;
const Poi = () => <div className="p-8"><h1>POI Keşfet Sayfası</h1></div>;
const Harita = () => <div className="p-8"><h1>Harita Sayfası</h1></div>;
const RotaOlustur = () => <div className="p-8"><h1>Rota Oluşturma Sayfası</h1></div>;


function App() {
  return (
    <Router>
      <div>
        {/* Navbar'ı buraya ekleyerek her sayfada görünmesini sağlayın */}
        <Navbar />

        {/* Sayfa içeriği burada değişecek */}
        <main>
          <Routes>
            <Route path="/" element={<AnaSayfa />} />
            <Route path="/routes" element={<Rotalar />} />
            <Route path="/poi" element={<Poi />} />
            <Route path="/map" element={<Harita />} />
            <Route path="/route-builder" element={<RotaOlustur />} />
            {/* Diğer sayfalarınız için Route'ları buraya ekleyebilirsiniz */}
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;