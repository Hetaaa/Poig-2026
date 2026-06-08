import { create } from "zustand";
import {
  getFavouriteOutfits,
  changeFavouriteStatus,
} from "./favouriteService";
import { getErrorMessage } from "../../utils/helpers";

const initialState = {
  favouriteOutfits: [],
  status: "idle",
  error: null,
};

export const useFavouriteOutfitStore = create((set, get) => ({
  ...initialState,

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

  async changeFavourite(outfit) {
    set({ status: "loading", error: null });

    try {
      const isFavourite = get().favouriteOutfits.some((fav) => fav.id === outfit.id);
      const newIsFavourite = !isFavourite;

      await changeFavouriteStatus(outfit.id, newIsFavourite);

      set((state) => ({
        favouriteOutfits: isFavourite
          ? state.favouriteOutfits.filter((fav) => fav.id !== outfit.id)
          : [...state.favouriteOutfits, outfit],
        status: "success",
        error: null,
      }));
      
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