import React from "react";
import { FaUserPlus, FaSignInAlt } from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const Footer = () => {
  const navigate = useNavigate();

  return (
    <footer className="w-full py-10 px-6 text-center bg-gradient-to-r from-cyan-500 to-blue-500 text-white shadow-2xl rounded-t-2xl mt-auto">
      <h2 className="text-2xl md:text-3xl font-bold mb-3">
        Kendi Rotanızı Oluşturmaya Hazır Mısınız?
      </h2>
      <p className="text-base md:text-lg mb-6 opacity-90">
        Ücretsiz hesap oluşturun ve sınırsız rota planlama özelliğinin keyfini çıkarın
      </p>
      <div className="flex justify-center gap-4 flex-wrap">
        <button
          onClick={() => navigate("/register")}
          className="flex items-center gap-2 bg-white text-blue-600 hover:bg-blue-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transform transition hover:scale-105 px-6 py-3 rounded-lg font-semibold shadow"
        >
          <FaUserPlus /> Ücretsiz Üye Ol
        </button>
        <button
          onClick={() => navigate("/login")}
          className="flex items-center gap-2 border border-white hover:bg-white hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transform transition hover:scale-105 px-6 py-3 rounded-lg font-semibold"
        >
          <FaSignInAlt /> Giriş Yap
        </button>
      </div>
      <p className="mt-6 text-sm opacity-75">© {new Date().getFullYear()} Gezenti</p>
    </footer>
  );
};

export default Footer;
