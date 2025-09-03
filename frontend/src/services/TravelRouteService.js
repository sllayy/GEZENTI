import axios from "axios";

const API_URL = process.env.REACT_APP_API_BASE_URL || "http://localhost:5136";

export async function createTravelRoute(payload) {
  const res = await axios.post(`${API_URL}/api/travelroutes`, payload);
  return res.data;
}

export async function getTravelRoutes() {
  const res = await axios.get(`${API_URL}/api/travelroutes`);
  return res.data;
}