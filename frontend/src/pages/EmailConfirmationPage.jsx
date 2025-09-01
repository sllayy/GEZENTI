import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { InputText } from 'primereact/inputtext';
import { authAPI } from '../services/api';

const EmailConfirmationPage = ({ setIsLoggedIn, setUserName }) => {
    const [searchParams] = useSearchParams();
    const [email, setEmail] = useState('');
    const [code, setCode] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        // URL'den email parametresini al
        const emailParam = searchParams.get('email');
        if (emailParam) {
            setEmail(emailParam);
        }
    }, [searchParams]);

    const handleConfirmEmail = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');
        setIsLoading(true);

        if (!email || !code) {
            setError('Lütfen e-posta ve doğrulama kodunu girin.');
            setIsLoading(false);
            return;
        }

        try {
            console.log('E-posta doğrulama işlemi başlatılıyor...');
            console.log('Gönderilen veriler:', { email, code });

            const response = await authAPI.confirmEmail({ email, code });
            console.log('E-posta doğrulama başarılı:', response);

            setSuccess('E-posta adresiniz başarıyla doğrulandı! Giriş yapabilirsiniz.');
            
            // 2 saniye sonra login sayfasına yönlendir
            setTimeout(() => {
                navigate('/login');
            }, 2000);

        } catch (err) {
            console.error('E-posta doğrulama sırasında hata:', err);
            
            if (err.response) {
                if (err.response.data && err.response.data.message) {
                    setError(err.response.data.message);
                } else {
                    setError('Doğrulama işlemi başarısız. Lütfen kodunuzu kontrol edin.');
                }
            } else {
                setError('Ağ hatası veya bilinmeyen bir sorun oluştu.');
            }
        } finally {
            setIsLoading(false);
        }
    };

    const handleResendCode = async () => {
        setError('');
        setIsLoading(true);

        try {
            const response = await authAPI.resendCode({
                email,
                purpose: 'ConfirmEmail'
            });
            
            setSuccess('Yeni doğrulama kodu e-posta adresinize gönderildi.');
        } catch (err) {
            console.error('Kod yeniden gönderme hatası:', err);
            if (err.response && err.response.data && err.response.data.message) {
                setError(err.response.data.message);
            } else {
                setError('Kod yeniden gönderilemedi. Lütfen tekrar deneyin.');
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
                    E-posta Doğrulama
                </h1>
                <p className="text-gray-600 mt-2">
                    Hesabınızı aktifleştirmek için e-posta adresinizi doğrulayın
                </p>
            </div>

            <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8 space-y-6">
                <div className="text-center">
                    <h2 className="text-2xl font-bold text-gray-900">Doğrulama Kodu</h2>
                    <p className="mt-1 text-sm text-gray-500">
                        E-posta adresinize gönderilen 6 haneli kodu girin
                    </p>
                </div>

                {error && (
                    <div className="p-4 text-sm text-red-700 bg-red-100 rounded-lg" role="alert">
                        {error}
                    </div>
                )}

                {success && (
                    <div className="p-4 text-sm text-green-700 bg-green-100 rounded-lg" role="alert">
                        {success}
                    </div>
                )}

                <form className="space-y-4" onSubmit={handleConfirmEmail}>
                    <div>
                        <label htmlFor="email" className="text-sm font-medium text-gray-700">
                            E-posta Adresi
                        </label>
                        <InputText
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full mt-1 p-inputtext-lg"
                            placeholder="ornek@email.com"
                            disabled={!!searchParams.get('email')}
                        />
                    </div>

                    <div>
                        <label htmlFor="code" className="text-sm font-medium text-gray-700">
                            Doğrulama Kodu
                        </label>
                        <InputText
                            id="code"
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            className="w-full mt-1 p-inputtext-lg"
                            placeholder="123456"
                            maxLength={6}
                        />
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
                                    Doğrulanıyor...
                                </>
                            ) : (
                                'E-postayı Doğrula'
                            )}
                        </button>
                    </div>
                </form>

                <div className="text-center">
                    <button
                        onClick={handleResendCode}
                        disabled={isLoading}
                        className="text-sm text-blue-600 hover:text-blue-500 underline disabled:text-gray-400"
                    >
                        Kod gelmedi mi? Yeniden gönder
                    </button>
                </div>

                <div className="text-center">
                    <p className="text-sm text-gray-600">
                        Zaten hesabınız var mı?{' '}
                        <button
                            onClick={() => navigate('/login')}
                            className="font-medium text-blue-600 hover:text-blue-500"
                        >
                            Giriş yapın
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default EmailConfirmationPage;
