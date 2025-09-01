import { initializeApp } from "firebase/app";
import { getAuth, GoogleAuthProvider } from "firebase/auth";

// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyAZRfHVjAixK5cAJ6rcq6hmiDvo5LMnDho",
    authDomain: "gezenti2-675f1.firebaseapp.com",
    projectId: "gezenti2-675f1",
    storageBucket: "gezenti2-675f1.firebasestorage.app",
    messagingSenderId: "48303852061",
    appId: "1:48303852061:web:784e839ba34ac4a10614f4",
    measurementId: "G-L2F0Y5CB4F"
};

// Firebase'i başlat
const app = initializeApp(firebaseConfig);

// Diğer React bileşenlerinde kullanmak üzere auth ve provider'ı export et
export const auth = getAuth(app);
export const googleProvider = new GoogleAuthProvider();