import { create } from "zustand";
import {
  getDailyWeather,
  setLastLocation as setLastLocationRequest,
} from "./weatherService";
import { getErrorMessage } from "../../../../utils/helpers";
import { formatDate } from "../../../../utils/dateUtil";

const initialState = {
  weather: null,
  status: "idle",
  error: null,
};

export const useWeatherStore = create((set) => ({
  ...initialState,
  async fetchDailyWeather(date = new Date()) {
    set({ status: "loading", error: null });
    try {
      const dateStr = formatDate(date);
      const data = await getDailyWeather(dateStr);
      set({ weather: data, status: "success" });
    } catch (error) {
      set({ error: getErrorMessage(error), status: "error" });
    }
  },
  async setLastLocation(location) {
    set({ status: "loading", error: null });
    try {
      await setLastLocationRequest(location);
      set({ status: "success" });
    } catch (error) {
      set({ error: getErrorMessage(error), status: "error" });
    }
  },
  clearWeather() {
    set(initialState);
  },
}));
