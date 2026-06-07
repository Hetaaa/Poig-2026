import { create } from "zustand";
import {
  getClothingItems,
  createClothingItem,
  updateClothingItem,
  deleteClothingItem,
} from "./clothingItemsService";
import { getErrorMessage } from "../../../utils/helpers";

const initialState = {
  clothingItems: [],
  status: "idle",
  error: null,
};

export const useClothingItemsStore = create((set) => ({
  ...initialState,

  async fetchClothingItems() {
    set({ status: "loading", error: null });

    try {
      const data = await getClothingItems();

      set({
        clothingItems: data,
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

  async addClothingItem(item) {
    set({ status: "loading", error: null });

    try {
      const createdItem = await createClothingItem(item);

      set((state) => ({
        clothingItems: [...state.clothingItems, createdItem],
        status: "success",
        error: null,
      }));

      return createdItem;
    } catch (error) {
      set({
        status: "error",
        error: getErrorMessage(error),
      });

      throw error;
    }
  },

  async updateClothingItem(id, item) {
    set({ status: "loading", error: null });
    try {
      await updateClothingItem(id, item);
      set((state) => ({
        clothingItems: state.clothingItems.map((ci) =>
          ci.id === id ? { ...ci, ...item } : ci,
        ),
        status: "success",
        error: null,
      }));
    } catch (error) {
      set({ status: "error", error: getErrorMessage(error) });
      throw error;
    }
  },

  async removeClothingItem(id) {
    // optimistic remove
    let removed;
    set((state) => {
      removed = state.clothingItems.find((item) => item.id === id);
      return {
        clothingItems: state.clothingItems.filter((item) => item.id !== id),
      };
    });

    try {
      await deleteClothingItem(id);
    } catch (error) {
      // rollback
      set((state) => ({
        clothingItems: removed
          ? [...state.clothingItems, removed]
          : state.clothingItems,
        error: getErrorMessage(error),
      }));
      throw error;
    }
  },

  resetClothingItems() {
    set(initialState);
  },

  clearError() {
    set({ error: null });
  },
}));
