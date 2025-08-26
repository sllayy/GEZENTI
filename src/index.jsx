import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css'; // Global CSS dosyamız
import App from './App'; // Ana Uygulama bileşenimiz

// Bu kod, React uygulamasını alır ve HTML'deki 'root' ID'li elementin içine yerleştirir.
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);