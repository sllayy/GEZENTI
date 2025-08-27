import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar';
import Footer from './components/Layout/Footer'; // Remote'dan eklenen Footer bileşeni
import Main from './components/Main/main'; // Remote'dan eklenen Main bileşeni
import MapComponent from './components/Map/MapComponent';

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
        <Navbar isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

        <main className="flex-grow">
          <Routes>
            {/* Ana sayfa ve Footer birlikte */}
            <Route
              path="/"
              element={
                <div>
                  <Main />
                  <Footer />
                </div>
              }
            />

            <Route path="/routes" element={<RoutesPage />} />
            <Route path="/poi" element={<DiscoverPoi />} />
            <Route path="/map" element={<MapComponent />} />
            <Route path="/route-builder" element={<RotaOlustur />} />
            <Route path="/login" element={<LoginPage setIsLoggedIn={setIsLoggedIn} />} />
            <Route path="/register" element={<RegisterPage setIsLoggedIn={setIsLoggedIn} />} />
          </Routes>
        </main>
      </div>
    </Router>
  );

}

export default App;
