import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { InputText } from 'primereact/inputtext';
import { Password } from 'primereact/password';
import { Checkbox } from 'primereact/checkbox';
import { authAPI } from '../services/api';

// Google ikonu için basit bir SVG bileşeni
const GoogleIcon = () => (
    <svg className="w-5 h-5 mr-2" viewBox="0 0 48 48">
        <path fill="#FFC107" d="M43.611,20.083H42V20H24v8h11.303c-1.649,4.657-6.08,8-11.303,8c-6.627,0-12-5.373-12-12s5.373-12,12-12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C12.955,4,4,12.955,4,24s8.955,20,20,20s20-8.955,20-20C44,22.659,43.862,21.35,43.611,20.083z"></path><path fill="#FF3D00" d="M6.306,14.691l6.571,4.819C14.655,15.108,18.961,12,24,12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C16.318,4,9.656,8.337,6.306,14.691z"></path><path fill="#4CAF50" d="M24,44c5.166,0,9.86-1.977,13.409-5.192l-6.19-5.238C29.211,35.091,26.715,36,24,36c-5.202,0-9.619-3.317-11.283-7.946l-6.522,5.025C9.505,39.556,16.227,44,24,44z"></path><path fill="#1976D2" d="M43.611,20.083H42V20H24v8h11.303c-0.792,2.237-2.231,4.166-4.087,5.574l6.19,5.238C39.988,35.69,44,30.169,44,24C44,22.659,43.862,21.35,43.611,20.083z"></path>
    </svg>
);

const RegisterPage = ({ setIsLoggedIn, setUserName }) => {
    const [fullName, setFullName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [agreedToTerms, setAgreedToTerms] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);
        
        // Form doğrulaması
        if (!fullName || !email || !password) {
            setError("Lütfen tüm zorunlu alanları doldurun.");
            setIsLoading(false);
            return;
        }
        if (password !== confirmPassword) {
            setError("Şifreler uyuşmuyor!");
            setIsLoading(false);
            return;
        }
        if (!agreedToTerms) {
            setError("Devam etmek için kullanım koşullarını kabul etmelisiniz.");
            setIsLoading(false);
            return;
        }

        // Ad ve soyadı ayır
        const nameParts = fullName.trim().split(' ');
        const firstName = nameParts[0] || '';
        const lastName = nameParts.slice(1).join(' ') || '';

        if (!firstName) {
            setError("Lütfen en az bir ad girin.");
            setIsLoading(false);
            return;
        }

        try {
            console.log("Kayıt işlemi başlatılıyor...");
            console.log("Gönderilen veriler:", { firstName, lastName, email, password });

            // API'ye kayıt isteği gönder
            const response = await authAPI.register({
                firstName,
                lastName,
                email,
                password
            });

            console.log("Kayıt başarılı:", response);

            // Başarılı kayıt sonrası işlemler - Kullanıcı henüz giriş yapmamış
            // E-posta doğrulama sayfasına yönlendir
            navigate(`/confirm-email?email=${encodeURIComponent(email)}`);
            
        } catch (err) {
            console.error('Kayıt işlemi sırasında hata:', err);
            
            if (err.response) {
                if (err.response.status === 400) {
                    if (err.response.data.message) {
                        setError(err.response.data.message);
                    } else if (err.response.data.errors) {
                        // Validation hatalarını birleştir
                        const errorMessages = Object.values(err.response.data.errors).flat();
                        setError(errorMessages.join(', '));
                    } else {
                        setError('Kayıt işlemi başarısız. Lütfen bilgilerinizi kontrol edin.');
                    }
                } else {
                    setError('Sunucu hatası: ' + err.response.status);
                }
            } else {
                setError('Ağ hatası veya bilinmeyen bir sorun oluştu.');
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-teal-50 via-cyan-50 to-blue-100 p-4 font-sans">
            
            <div className="text-center mb-8">
                <img src="/LOGO2.png" alt="Gezenti Logo" className="h-16 w-16 mx-auto mb-4 rounded-full shadow-lg" />
                <h1 className="text-4xl font-bold text-gray-800">
                    Gezenti'ye Katılın
                </h1>
                <p className="text-gray-600 mt-2">
                    Yeni bir hesap oluşturarak maceraya ortak olun.
                </p>
            </div>

            <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8 space-y-6">
                <div className="text-center">
                    <h2 className="text-2xl font-bold text-gray-900">Hesap Oluştur</h2>
                    <p className="mt-1 text-sm text-gray-500">
                       Bilgilerinizi girerek hızlıca kaydolun
                    </p>
                </div>

                {error && (
                    <div className="p-4 text-sm text-red-700 bg-red-100 rounded-lg" role="alert">
                        {error}
                    </div>
                )}
                
                <form className="space-y-4" onSubmit={handleRegister}>
                    <div>
                        <label htmlFor="fullName" className="text-sm font-medium text-gray-700">Ad Soyad</label>
                        <InputText id="fullName" value={fullName} onChange={(e) => setFullName(e.target.value)} className="w-full mt-1 p-inputtext-lg" placeholder="Adınız Soyadınız"/>
                    </div>
                    
                    <div>
                        <label htmlFor="email" className="text-sm font-medium text-gray-700">E-posta</label>
                        <InputText id="email" value={email} onChange={(e) => setEmail(e.target.value)} className="w-full mt-1 p-inputtext-lg" placeholder="ornek@email.com"/>
                    </div>

                    <div>
                        <label htmlFor="password" className="text-sm font-medium text-gray-700">Şifre</label>
                        <Password inputId="password" value={password} onChange={(e) => setPassword(e.target.value)} className="w-full mt-1" inputClassName="w-full p-inputtext-lg" placeholder="********" feedback={false} toggleMask/>
                    </div>

                     <div>
                        <label htmlFor="confirmPassword" className="text-sm font-medium text-gray-700">Şifre Tekrar</label>
                        <Password inputId="confirmPassword" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} className="w-full mt-1" inputClassName="w-full p-inputtext-lg" placeholder="********" feedback={false} toggleMask/>
                    </div>

                    <div className="flex items-start pt-2">
                        <div className="flex items-center h-5">
                            <Checkbox inputId="terms" onChange={e => setAgreedToTerms(e.checked)} checked={agreedToTerms}></Checkbox>
                        </div>
                        <div className="ml-3 text-sm">
                            <label htmlFor="terms" className="text-gray-600">
                                <a href="#" className="font-medium text-blue-600 hover:underline">Kullanım Koşulları</a>'nı ve <a href="#" className="font-medium text-blue-600 hover:underline">Gizlilik Politikası</a>'nı kabul ediyorum.
                            </label>
                        </div>
                    </div>
                    
                    <div className="pt-2">
                         <button 
                            type="submit" 
                            disabled={isLoading}
                            className={`w-full flex justify-center items-center px-4 py-3 border border-transparent text-base font-medium rounded-md text-white transition-transform transform hover:scale-105 ${
                                isLoading 
                                    ? 'bg-gray-400 cursor-not-allowed' 
                                    : 'bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600'
                            }`}
                        >
                            {isLoading ? (
                                <>
                                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Kayıt Yapılıyor...
                                </>
                            ) : (
                                'Hesap Oluştur'
                            )}
                        </button>
                    </div>
                </form>

                <div className="relative my-4">
                    <div className="absolute inset-0 flex items-center"><div className="w-full border-t border-gray-300" /></div>
                    <div className="relative flex justify-center text-sm"><span className="px-2 bg-white text-gray-500">VEYA</span></div>
                </div>

                <div>
                    <button className="w-full inline-flex justify-center items-center py-3 px-4 border border-gray-300 rounded-md shadow-sm bg-white text-base font-medium text-gray-700 hover:bg-gray-50 transition-colors">
                        <GoogleIcon /> Google ile Kaydol
                    </button>
                </div>

                <p className="text-sm text-center text-gray-600">
                    Zaten bir hesabınız var mı?{' '}
                    <Link to="/login" className="font-medium text-blue-600 hover:text-blue-500">
                        Giriş yapın
                    </Link>
                </p>
            </div>
        </div>
    );
};

export default RegisterPage;