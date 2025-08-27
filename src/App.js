import React from 'react';
import Navbar from './components/Layout/Navbar';
import Footer from './components/Layout/Footer';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Main from './components/Main/main';
import DiscoverPoi from './components/Discover_poi/DiscoverPoi';

const Rotalar = () => (
  <div className="p-8">
    <h1>Rotalar Sayfası</h1>
  </div>
);

const Harita = () => (
  <div className="p-8">
    <h1>Harita Sayfası</h1>
  </div>
);

const RotaOlustur = () => (
  <div className="p-8">
    <h1>Rota Oluşturma Sayfası</h1>
  </div>
);

function App() {
  return (
    <Router>
      <div className="flex flex-col min-h-screen">
        {/* Navbar her sayfada görünecek */}
        <Navbar />

        {/* Sayfa içeriği */}
        <main className="flex-grow">
          <Routes>
            <Route
              path="/"
              element={
                <>
                  <Main />
                  <Footer /> {/* Sadece ana sayfada göster */}
                </>
              }
            />
            <Route path="/routes" element={<Rotalar />} />
            <Route path="/poi" element={<DiscoverPoi />} />
            <Route path="/map" element={<Harita />} />
            <Route path="/route-builder" element={<RotaOlustur />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
