// src/index.js

import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css'; // Bu satır muhtemelen zaten vardır
import App from './App';
import reportWebVitals from './reportWebVitals';
import './index.css';
// PrimeReact stillerini buraya ekleyin
import "primereact/resources/themes/saga-blue/theme.css";  // Tema dosyası (saga-blue, arya-blue, vb. seçebilirsiniz)
import "primereact/resources/primereact.min.css";          // PrimeReact'in çekirdek CSS'i
import 'primeicons/primeicons.css'; // Eksik olan ikon stilleri


const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);


reportWebVitals();