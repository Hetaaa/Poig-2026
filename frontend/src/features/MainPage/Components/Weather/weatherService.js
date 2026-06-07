import { apiClient } from "../../../../api/apiClient";

export async function getDailyWeather(date) {
  const response = await apiClient.get(`/WeatherPicker/daily/${date}`);
  return response.data;
}

export async function getLastLocation() {
  const response = await apiClient.get("/WeatherPicker/last");
  return response.data;
}

export async function setLastLocation(location) {
  const response = await apiClient.post("/WeatherPicker/pick", location);
  return response.data;
}
