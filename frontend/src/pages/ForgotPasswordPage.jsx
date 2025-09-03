import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { getApiUrl } from "../config/environment";

const ForgotPasswordPage = () => {
    const [email, setEmail] = useState("");
    const [code, setCode] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [step, setStep] = useState(1); // 1: kod gönderme, 2: şifre sıfırlama
    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    const handleSendCode = async (e) => {
        e.preventDefault();
        setMessage("");

        try {
            const res = await axios.post(`${getApiUrl()}/auth/forgot-password`, { email });
            setMessage(res.data.message || "Eğer kayıtlıysa mail gönderildi.");
            setStep(2); // kod gönderildikten sonra şifre sıfırlama adımına geç
        } catch (err) {
            setMessage("Kod gönderilemedi. Tekrar deneyin.");
        }
    };

    const handleResetPassword = async (e) => {
        e.preventDefault();
        setMessage("");

        // Şifre kontrolü
        if (newPassword !== confirmPassword) {
            setMessage("Şifreler uyuşmuyor!");
            return;
        }

        // Şifre validasyonu
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,12}$/;
        if (!passwordRegex.test(newPassword)) {
            setMessage("Şifre 8-12 karakter olmalı ve en az 1 büyük harf, 1 küçük harf, 1 rakam ve 1 sembol içermelidir.");
            return;
        }

        try {
            const res = await axios.post(`${getApiUrl()}/auth/reset-password`, {
                email,
                code,
                newPassword,
            });
            setMessage(res.data.message || "Şifre sıfırlandı ✅");

            // 5 saniye sonra login sayfasına yönlendir
            setTimeout(() => {
                navigate("/login");
            }, 5000);
        } catch (err) {
            if (err.response?.data?.message) {
                setMessage(err.response.data.message);
            } else {
                setMessage("Şifre sıfırlanamadı. Tekrar deneyin.");
            }
        }
    };

    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-cyan-50 via-blue-50 to-indigo-100 p-4">
            <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8 space-y-6">
                <h2 className="text-2xl font-bold text-center text-gray-900">Şifremi Unuttum</h2>
                <p className="text-gray-600 text-center text-sm">
                    {step === 1
                        ? "E-posta adresinizi girin, size sıfırlama kodu gönderelim."
                        : "Mailinize gelen kodu ve yeni şifrenizi girin."}
                </p>

                {message && (
                    <div className="p-3 text-sm text-center rounded-md bg-gray-100 text-gray-800">
                        {message}
                    </div>
                )}

                {step === 1 && (
                    <form className="space-y-4" onSubmit={handleSendCode}>
                        <input
                            type="email"
                            placeholder="E-posta"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full border p-2 rounded"
                            required
                        />
                        <button
                            type="submit"
                            className="w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600 transition"
                        >
                            Sıfırlama Kodu Gönder
                        </button>
                    </form>
                )}

                {step === 2 && (
                    <form className="space-y-4" onSubmit={handleResetPassword}>
                        <input
                            type="text"
                            placeholder="Doğrulama Kodu"
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            className="w-full border p-2 rounded"
                            required
                        />
                        <input
                            type="password"
                            placeholder="Yeni Şifre"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            className="w-full border p-2 rounded"
                            required
                        />
                        <input
                            type="password"
                            placeholder="Yeni Şifre Tekrar"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            className="w-full border p-2 rounded"
                            required
                        />
                        <button
                            type="submit"
                            className="w-full bg-green-500 text-white py-2 rounded hover:bg-green-600 transition"
                        >
                            Şifreyi Sıfırla
                        </button>
                    </form>
                )}
            </div>
        </div>
    );
};

export default ForgotPasswordPage;
