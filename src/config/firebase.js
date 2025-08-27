import { initializeApp } from "firebase/app";
import { getAuth, GoogleAuthProvider } from "firebase/auth";

// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyB0yitPa2tU8kY391MslBHRYZ5fV3R6Cf8",
  authDomain: "geziproject2.firebaseapp.com",
  projectId: "geziproject2",
  storageBucket: "geziproject2.firebasestorage.app",
  messagingSenderId: "412024861292",
  appId: "1:412024861292:web:05dec515595dc640abde49",
  measurementId: "G-P0RN1D8LCK"
};

// Firebase'i başlat
const app = initializeApp(firebaseConfig);

// Diğer React bileşenlerinde kullanmak üzere auth ve provider'ı export et
export const auth = getAuth(app);
export const googleProvider = new GoogleAuthProvider();