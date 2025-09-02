import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./index.css";
import { ThemeProvider } from "./context/ThemeContext"; // ✅ ekle

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <React.StrictMode>
    <ThemeProvider> {/* ✅ tüm uygulamayı sarmala */}
      <App />
    </ThemeProvider>
  </React.StrictMode>
);
