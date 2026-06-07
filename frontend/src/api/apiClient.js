import axios from "axios";

const backendPort = 5267;
const authTokenKey = "authToken";

export const backendBaseUrl = `http://127.0.0.1:${backendPort}`;

export const apiClient = axios.create({
  baseURL: `${backendBaseUrl}/api`,
  timeout: 5000,
});

function getAuthToken() {
  return localStorage.getItem(authTokenKey) || localStorage.getItem("token");
}

export function setAuthToken(token) {
  if (token) {
    localStorage.setItem(authTokenKey, token);
  } else {
    localStorage.removeItem(authTokenKey);
  }
}

apiClient.interceptors.request.use((config) => {
  const token = getAuthToken();
  if (token) {
    config.headers = config.headers || {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
