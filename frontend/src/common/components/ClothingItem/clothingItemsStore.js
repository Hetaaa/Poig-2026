import { create } from "zustand";
import { getClothingItems, 
    createClothingItem
} from "./clothingItemsService";

const initialState = {
  clothingItems: [],
  status: "idle",
  error: null,
};

function getErrorMessage(error) {
  return (
    error?.response?.data?.message ||
    error?.message ||
    "Nie udało się pobrać ubrań"
  );
}

export const useClothingItemsStore = create((set) => ({
  ...initialState,

  async fetchClothingItems() {
    set({ status: "loading", error: null });

    try {
      const data = await getClothingItems();
      console.log("DATA Z BACKENDU:", data);
        console.log("Czy to tablica?", Array.isArray(data));

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
    set({status: "loading", error: null});

    try { 
        const createdItem = await createClothingItem(item);

        set((state)=> ({
            clothingItems: [...state.clothingItems, createdItem],
            status: "success", 
            error: null,
        }));

        return createdItem; 
    } catch(error) {
        set({
            status: "error", 
            error: getErrorMessage(error),
        });

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