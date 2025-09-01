import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

// Layout
import Navbar from "./components/Layout/Navbar";
import Footer from "./components/Layout/Footer";

// Pages
import Main from "./components/Main/main";
import MapComponent from "./pages/MapComponentPage.jsx";
import ProfilePage from "./pages/ProfilePage";
import RouteBuilderPage from "./pages/RouteBuilderPage";
import NotFound from "./pages/NotFound";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import EmailConfirmationPage from "./pages/EmailConfirmationPage.jsx";
import RoutesPage from "./pages/RoutesPage.jsx";
import DiscoverPoi from "./pages/DiscoverPoiPage.jsx";

// Development Port Config
import PortConfig from "./components/DevConfig/PortConfig";

// PrimeReact styles
import "primereact/resources/themes/saga-blue/theme.css";
import "primereact/resources/primereact.min.css";
import "primeicons/primeicons.css";

// Auth
import PrivateRoute from "./components/Auth/PrivateRoute";

function App() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userName, setUserName] = useState("");

    // Sayfa yüklendiğinde token kontrolü
    useEffect(() => {
        const token = localStorage.getItem("jwtToken");
        if (token) {
            setIsLoggedIn(true);

            // Token varsa kullanıcı adını da localStorage'dan al
            const savedUserName = localStorage.getItem("userName");
            if (savedUserName) {
                setUserName(savedUserName);
            }
        }
    }, []);

    return (
        <Router>
            <div className="flex flex-col min-h-screen">
                {/* Navbar'a userName'i de gönderdik */}
                <Navbar
                    isLoggedIn={isLoggedIn}
                    setIsLoggedIn={setIsLoggedIn}
                    userName={userName}
                />

                <main className="flex-grow">
                    <Routes>
                        {/* Ana sayfa herkese açık */}
                        <Route
                            path="/"
                            element={
                                <div>
                                    <Main />
                                    <Footer />
                                </div>
                            }
                        />

                        {/* Public Auth Sayfaları */}
                        <Route
                            path="/login"
                            element={
                                <LoginPage
                                    setIsLoggedIn={setIsLoggedIn}
                                    setUserName={setUserName}
                                />
                            }
                        />
                        <Route
                            path="/register"
                            element={
                                <RegisterPage
                                    setIsLoggedIn={setIsLoggedIn}
                                    setUserName={setUserName}
                                />
                            }
                        />
                        <Route
                            path="/confirm-email"
                            element={
                                <EmailConfirmationPage
                                    setIsLoggedIn={setIsLoggedIn}
                                    setUserName={setUserName}
                                />
                            }
                        />

                        {/* Private Sayfalar */}
                        <Route
                            path="/routes"
                            element={
                                <PrivateRoute isLoggedIn={isLoggedIn}>
                                    <RoutesPage />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/poi"
                            element={
                                <PrivateRoute isLoggedIn={isLoggedIn}>
                                    <DiscoverPoi />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/map"
                            element={
                                <PrivateRoute isLoggedIn={isLoggedIn}>
                                    <MapComponent />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/profile"
                            element={
                                <PrivateRoute isLoggedIn={isLoggedIn}>
                                    <ProfilePage />
                                </PrivateRoute>
                            }
                        />
                        <Route
                            path="/route-builder"
                            element={
                                <PrivateRoute isLoggedIn={isLoggedIn}>
                                    <RouteBuilderPage />
                                </PrivateRoute>
                            }
                        />

                        {/* 404 */}
                        <Route path="*" element={<NotFound />} />
                    </Routes>
                </main>

                {/* Development Port Configuration Component */}
                <PortConfig />
            </div>
        </Router>
    );
}

export default App;
