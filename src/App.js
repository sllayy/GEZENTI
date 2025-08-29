import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Layout/Navbar';
import Footer from './components/Layout/Footer';
import Main from './components/Main/main';
import MapComponent from './pages/MapComponentPage.jsx';
import ProfilePage from './pages/ProfilePage';
import RouteBuilderPage from './pages/RouteBuilderPage';
import NotFound from './pages/NotFound';
// Sayfalar
import LoginPage from './pages/LoginPage.jsx';
import RegisterPage from './pages/RegisterPage.jsx';
import RoutesPage from './pages/RoutesPage.jsx';
import DiscoverPoi from './pages/DiscoverPoiPage.jsx';
// PrimeReact stil dosyaları (eğer henüz eklenmediyse)
import 'primereact/resources/themes/saga-blue/theme.css'; // Seçtiğiniz tema
import 'primereact/resources/primereact.min.css'; // PrimeReact ana CSS
import 'primeicons/primeicons.css'; // PrimeIcons
function App() {
  
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  
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

            
            <Route path="/login" element={<LoginPage setIsLoggedIn={setIsLoggedIn} setUserName={setUserName} />} />
            <Route path="/register" element={<RegisterPage setIsLoggedIn={setIsLoggedIn} setUserName={setUserName} />} />

            <Route path="/profile" element={isLoggedIn ? <ProfilePage /> : <LoginPage setIsLoggedIn={setIsLoggedIn} setUserName={setUserName} />} />
            <Route path="/route-builder" element={<RouteBuilderPage />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
