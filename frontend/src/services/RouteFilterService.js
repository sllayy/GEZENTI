import axios from "axios";

const API_URL = "https://localhost:7248/api/RouteFilter";

export const filterRoutes = async (filters) => {
    try {
        const body = {
            category: filters.selectedCategory || null,
            averageRating: filters.ratingValue,
            distanceKm: filters.distanceValue,
            duration: filters.selectedDuration
        };

        console.log("Filtre backend'e gönderiliyor:", body);

        const response = await axios.post(`${API_URL}/filter`, body);
        return response.data;
    } catch (error) {
        console.error("Filtre uygulama hatası:", error);
        throw error;
    }
};

export const addRoute = async (route) => {
    try {
        const response = await axios.post(`${API_URL}/add`, route);
        return response.data;
    } catch (error) {
        console.error("Rota ekleme hatası:", error);
        throw error;
    }
};
