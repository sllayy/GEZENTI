import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar';
import Footer from './components/Layout/Footer';
import Main from './components/Main/main';
import MapComponent from './pages/MapComponentPage.jsx';
import ProfilePage from './pages/ProfilePage';
import RouteBuilderPage from './pages/RouteBuilderPage';

// Sayfalar
import LoginPage from './pages/LoginPage.js';
import RegisterPage from './pages/RegisterPage.js';
import RoutesPage from './pages/RoutesPage';
import DiscoverPoi from './pages/DiscoverPoiPage.jsx';


function App() {
  // Giriş durumunu App bileşeninde tutuyoruz
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  // Kullanıcı adını App bileşeninde tutuyoruz
  const [userName, setUserName] = useState("");

  return (
    <Router>
      <div className="flex flex-col min-h-screen">
        {/* Navbar'a userName'i de gönderdik */}
        <Navbar isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} userName={userName} />

        <main className="flex-grow">
          <Routes>
            <Route path="/" element={
              <div>
                <Main />
                <Footer />
              </div>
            } />
            <Route path="/routes" element={<RoutesPage />} />
            <Route path="/poi" element={<DiscoverPoi />} />
            <Route path="/map" element={<MapComponent />} />

            {/* Login ve Register sayfalarına setUserName'i de gönderiyoruz */}
            <Route path="/login" element={<LoginPage setIsLoggedIn={setIsLoggedIn} setUserName={setUserName} />} />
            <Route path="/register" element={<RegisterPage setIsLoggedIn={setIsLoggedIn} setUserName={setUserName} />} />

            <Route path="/profile" element={isLoggedIn ? <ProfilePage /> : <LoginPage setIsLoggedIn={setIsLoggedIn} setUserName={setUserName} />} />
            <Route path="/route-builder" element={<RouteBuilderPage />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
