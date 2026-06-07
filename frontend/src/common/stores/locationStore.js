import { create } from "zustand";
import { apiClient } from "../../api/apiClient";
import { getErrorMessage } from "../../utils/helpers";

const initialState = {
  location: null, // { latitude, longitude, cityName }
  status: "idle",
  error: null,
};

export const useLocationStore = create((set) => ({
  ...initialState,

  async fetchLastLocation() {
    set({ status: "loading", error: null });
    try {
      const response = await apiClient.get("/WeatherPicker/last");
      set({ location: response.data, status: "success", error: null });
      return response.data;
    } catch (error) {
      if (error?.response?.status === 404) {
        set({ location: null, status: "not-set", error: null });
        return null;
      }
      set({ status: "error", error: getErrorMessage(error) });
      throw error;
    }
  },

  async saveLocation(latitude, longitude, cityName = null) {
    set({ status: "loading", error: null });
    try {
      await apiClient.post("/WeatherPicker/pick", { latitude, longitude });
      set({
        location: { latitude, longitude, cityName },
        status: "success",
        error: null,
      });
    } catch (error) {
      set({ status: "error", error: getErrorMessage(error) });
      throw error;
    }
  },

  reset() {
    set(initialState);
  },
}));
