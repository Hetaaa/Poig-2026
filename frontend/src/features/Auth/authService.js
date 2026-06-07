import { apiClient, setAuthToken } from "../../api/apiClient";

const authTokenKey = "authToken";
const authUsernameKey = "authUsername";

export function getStoredToken() {
  return localStorage.getItem(authTokenKey) || localStorage.getItem("token");
}

export function getStoredUsername() {
  return localStorage.getItem(authUsernameKey);
}

export function setStoredUsername(username) {
  if (username) {
    localStorage.setItem(authUsernameKey, username);
  } else {
    localStorage.removeItem(authUsernameKey);
  }
}

export async function login(username, password) {
  const response = await apiClient.post("/Auth/login", { username, password });
  const token = response?.data?.token;
  const storedUsername = response?.data?.username || response?.data?.Username;
  if (token) setAuthToken(token);
  if (storedUsername) setStoredUsername(storedUsername);
  return response.data;
}

export async function register(username, password) {
  const response = await apiClient.post("/Auth/register", {
    username,
    password,
  });
  const token = response?.data?.token;
  const storedUsername = response?.data?.username || response?.data?.Username;
  if (token) setAuthToken(token);
  if (storedUsername) setStoredUsername(storedUsername);
  return response.data;
}

export function logout() {
  setAuthToken(null);
  setStoredUsername(null);
  localStorage.removeItem("token");
}
