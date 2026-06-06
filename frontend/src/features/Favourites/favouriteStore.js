import { create } from "zustand";
import {
  getFavouriteOutfits,
} from "./favouriteService";
import { getErrorMessage } from "../../utils/helpers";

const initialState = {
  favouriteOutfits: [],
  status: "idle",
  error: null,
};

export const useFavouriteOutfitStore = create((set) => ({
  ...initialState,

  async addFavouriteOutfit(outfit) {
    set((state) => ({
      favouriteOutfits: [...state.favouriteOutfits, outfit],
    }));
  },

  async fetchFavouriteOutfits() {
    set({ status: "loading", error: null });
    try {
      const data = await getFavouriteOutfits();

      set({
        favouriteOutfits: data,
        status: "success",
        error: null,
      });

      return data;
    } catch (error) {
      set({
        status: "error",
        error: getErrorMessage(error),
      });

      throw error;
    }
  },

  resetFavouriteOutfits() {
    set(initialState);
  },

  clearError() {
    set({ error: null });
  },
}));