import React from "react";
import { FaUserPlus, FaSignInAlt } from "react-icons/fa";

const Footer = () => {
    return (
        <div className="w-full py-16 px-6 text-center bg-gradient-to-r from-cyan-500 to-blue-500 text-white rounded-t-2xl shadow-md">
            <h2 className="text-3xl md:text-4xl font-bold mb-4 ">
                Kendi Rotanızı Oluşturmaya Hazır Mısınız?
            </h2>
            <p className="text-lg md:text-xl mb-8">
                Ücretsiz hesap oluşturun ve sınırsız rota planlama özelliğinin keyfini çıkarın
            </p>
            <div className="flex justify-center gap-4 flex-wrap">
                <button className="flex items-center gap-2 bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-transform transform hover:scale-105 px-6 py-3 rounded-lg font-semibold transition">
                    <FaUserPlus /> Ücretsiz Üye Ol
                </button>
                <button className="flex items-center gap-2 border border-white bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-transform transform hover:scale-105 px-6 py-3 rounded-lg font-semibold transition">
                    <FaSignInAlt /> Giriş Yap
                </button>
            </div>
        </div>
    );
};

export default Footer;
