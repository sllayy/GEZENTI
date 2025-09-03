import React from "react";
import { Navigate } from "react-router-dom";

const PrivateRoute = ({ isLoggedIn, children }) => {
    const token = localStorage.getItem("jwtToken");

    if (!isLoggedIn && !token) {
        return <Navigate to="/login" replace />;
    }

    return children;
};

export default PrivateRoute;