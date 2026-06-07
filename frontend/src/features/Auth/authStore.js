import { create } from "zustand";
import {
  getStoredToken,
  getStoredUsername,
  login as loginService,
  logout as logoutService,
  register as registerService,
  setStoredUsername,
} from "./authService";

const initialState = {
  token: getStoredToken(),
  username: getStoredUsername(),
  status: "idle",
  error: null,
};

function getErrorMessage(error, isRegister) {
  return (
    error?.response?.data?.message ||
    error?.message ||
    (isRegister ? "Nie udalo sie utworzyc konta" : "Nie udalo sie zalogowac")
  );
}

export const useAuthStore = create((set) => ({
  ...initialState,
  async login(username, password) {
    set({ status: "loading", error: null });
    try {
      const data = await loginService(username, password);
      const resolvedUsername =
        data?.username || data?.Username || getStoredUsername() || username;
      setStoredUsername(resolvedUsername);
      set({
        token: data?.token || getStoredToken(),
        username: resolvedUsername,
        status: "success",
        error: null,
      });
      return data;
    } catch (error) {
      set({ status: "error", error: getErrorMessage(error, false) });
      throw error;
    }
  },
  async register(username, password) {
    set({ status: "loading", error: null });
    try {
      const data = await registerService(username, password);
      const resolvedUsername =
        data?.username || data?.Username || getStoredUsername() || username;
      setStoredUsername(resolvedUsername);
      set({
        token: data?.token || getStoredToken(),
        username: resolvedUsername,
        status: "success",
        error: null,
      });
      return data;
    } catch (error) {
      set({ status: "error", error: getErrorMessage(error, true) });
      throw error;
    }
  },
  logout() {
    logoutService();
    set({
      token: null,
      username: null,
      status: "idle",
      error: null,
    });
  },
  clearError() {
    set({ error: null });
  },
}));
