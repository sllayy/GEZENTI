import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { InputText } from 'primereact/inputtext';
import { Password } from 'primereact/password';
import { Checkbox } from 'primereact/checkbox';

// Google ikonu için basit bir SVG bileşeni
const GoogleIcon = () => (
    <svg className="w-5 h-5 mr-2" viewBox="0 0 48 48">
        <path fill="#FFC107" d="M43.611,20.083H42V20H24v8h11.303c-1.649,4.657-6.08,8-11.303,8c-6.627,0-12-5.373-12-12s5.373-12,12-12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C12.955,4,4,12.955,4,24s8.955,20,20,20s20-8.955,20-20C44,22.659,43.862,21.35,43.611,20.083z"></path>
        <path fill="#FF3D00" d="M6.306,14.691l6.571,4.819C14.655,15.108,18.961,12,24,12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C16.318,4,9.656,8.337,6.306,14.691z"></path>
        <path fill="#4CAF50" d="M24,44c5.166,0,9.86-1.977,13.409-5.192l-6.19-5.238C29.211,35.091,26.715,36,24,36c-5.202,0-9.619-3.317-11.283-7.946l-6.522,5.025C9.505,39.556,16.227,44,24,44z"></path>
        <path fill="#1976D2" d="M43.611,20.083H42V20H24v8h11.303c-0.792,2.237-2.231,4.166-4.087,5.574l6.19,5.238C39.988,35.69,44,30.169,44,24C44,22.659,43.862,21.35,43.611,20.083z"></path>
    </svg>
);

// App.js'ten prop olarak setIsLoggedIn fonksiyonunu alıyoruz.
const LoginPage = ({ setIsLoggedIn }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [checked, setChecked] = useState(false);
    const navigate = useNavigate();

    const handleLogin = (e) => {
        e.preventDefault(); // Formun sayfayı yenilemesini engelle
        if (email === "test@gezenti.com" && password === "12345") {
            setIsLoggedIn(true);
            navigate('/');
        } else {
            alert("Hatalı e-posta veya şifre!");
        }
    };

    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-teal-50 via-cyan-50 to-blue-100 p-4 font-sans">
            
            <div className="text-center mb-8">
                <img src="/LOGO2.png" alt="Gezenti Logo" className="h-16 w-16 mx-auto mb-4 rounded-full shadow-lg" />
                <h1 className="text-4xl font-bold text-gray-800">
                    Gezenti'ye Hoş Geldiniz
                </h1>
                <p className="text-gray-600 mt-2">
                    Macera dolu rotalar keşfetmeye hazır mısınız?
                </p>
            </div>

            <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8 space-y-6">
                <div className="text-center">
                    <h2 className="text-2xl font-bold text-gray-900">Giriş Yap</h2>
                    <p className="mt-1 text-sm text-gray-500">
                        Hesabınıza giriş yapın ve rotalarınızı keşfetmeye devam edin
                    </p>
                </div>
                
                <form className="space-y-6" onSubmit={handleLogin}>
                    <div>
                        <label htmlFor="email" className="text-sm font-medium text-gray-700">
                            E-posta
                        </label>
                        <InputText 
                            id="email" 
                            value={email} 
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full mt-1 p-inputtext-lg"
                            placeholder="ornek@email.com"
                        />
                    </div>

                    <div>
                        <label htmlFor="password" className="text-sm font-medium text-gray-700">
                            Şifre
                        </label>
                        <Password 
                            inputId="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            className="w-full mt-1"
                            inputClassName="w-full p-inputtext-lg"
                            placeholder="********"
                            feedback={false}
                            toggleMask
                        />
                    </div>

                    <div className="flex items-center justify-between text-sm">
                        <div className="flex items-center">
                            <Checkbox inputId="rememberMe" onChange={e => setChecked(e.checked)} checked={checked}></Checkbox>
                            <label htmlFor="rememberMe" className="ml-2 text-gray-600 cursor-pointer">Beni hatırla</label>
                        </div>
                        <a href="#" className="font-medium text-blue-600 hover:text-blue-500">
                            Şifremi unuttum
                        </a>
                    </div>
                    
                    <div>
                         <button type="submit" className="w-full flex justify-center items-center px-4 py-3 border border-transparent text-base font-medium rounded-md text-white bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-transform transform hover:scale-105">
                            Giriş Yap
                        </button>
                    </div>
                </form>

                <div className="relative my-4">
                    <div className="absolute inset-0 flex items-center">
                        <div className="w-full border-t border-gray-300" />
                    </div>
                    <div className="relative flex justify-center text-sm">
                        <span className="px-2 bg-white text-gray-500">VEYA</span>
                    </div>
                </div>


                <div>
                    <button className="w-full inline-flex justify-center items-center py-3 px-4 border border-gray-300 rounded-md shadow-sm bg-white text-base font-medium text-gray-700 hover:bg-gray-50 transition-colors">
                        <GoogleIcon /> Google ile Giriş
                    </button>
                </div>

                <p className="text-sm text-center text-gray-600">
                    Hesabınız yok mu?{' '}
                    <Link to="/register" className="font-medium text-blue-600 hover:text-blue-500">
                        Üye olun
                    </Link>
                </p>
            </div>

            <div className="text-center mt-8 text-xs text-gray-500">
                Giriş yaparak <a href="#" className="underline hover:text-gray-700">Kullanım Koşulları</a>'nı ve <a href="#" className="underline hover:text-gray-700">Gizlilik Politikası</a>'nı kabul etmiş olursunuz.
            </div>
        </div>
    );
};

export default LoginPage;