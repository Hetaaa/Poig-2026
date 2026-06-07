import { create } from "zustand";
import { getTodayOutfit, getOutfitsByRange, regenerateTodayOutfit } from "./outfitService";
import { getErrorMessage } from "../../../../utils/helpers";

const initialState = {
  todayOutfit: null,
  todayWarnings: [],
  todayStatus: "idle",
  todayError: null,
  upcomingOutfits: [],
  upcomingStatus: "idle",
  upcomingError: null,
};

export const useOutfitStore = create((set) => ({
  ...initialState,

  async fetchTodayOutfit() {
    set({ todayStatus: "loading", todayError: null });
    try {
      const data = await getTodayOutfit();
      set({
        todayOutfit: data.outfit ?? null,
        todayWarnings: data.warnings ?? [],
        todayStatus: "success",
        todayError: null,
      });
    } catch (error) {
      set({ todayStatus: "error", todayError: getErrorMessage(error) });
    }
  },

  async regenerateTodayOutfit() {
    set({ todayStatus: "loading", todayError: null });
    try {
      const data = await regenerateTodayOutfit();
      set({
        todayOutfit: data.outfit ?? null,
        todayWarnings: data.warnings ?? [],
        todayStatus: "success",
        todayError: null,
      });
    } catch (error) {
      set({ todayStatus: "error", todayError: getErrorMessage(error) });
    }
  },

  async fetchUpcomingOutfits(daysAhead = 3) {
    set({ upcomingStatus: "loading", upcomingError: null });
    try {
      const from = new Date();
      from.setDate(from.getDate() + 1);
      const to = new Date();
      to.setDate(to.getDate() + daysAhead);
      const data = await getOutfitsByRange(from, to);
      set({
        upcomingOutfits: data,
        upcomingStatus: "success",
        upcomingError: null,
      });
    } catch (error) {
      set({ upcomingStatus: "error", upcomingError: getErrorMessage(error) });
    }
  },

  reset() {
    set(initialState);
  },
}));
