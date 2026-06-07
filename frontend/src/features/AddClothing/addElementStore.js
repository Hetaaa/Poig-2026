import { create } from "zustand";

export const useAddElementStore = create((set)=> ({
    showAdd: false, 
    openAdd: () => set({showAdd: true}),
    closeAdd: () => set({showAdd: false}),
}));