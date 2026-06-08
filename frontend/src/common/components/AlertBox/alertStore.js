import { create } from "zustand";

let _dismissTimer = null;

export const useAlertStore = create((set, get) => ({
  message: null,
  type: "info",
  visible: false,
  alertKey: 0,

  showAlert(message, type = "info") {
    if (_dismissTimer) {
      clearTimeout(_dismissTimer);
      _dismissTimer = null;
    }
    set((s) => ({ message, type, visible: true, alertKey: s.alertKey + 1 }));
    _dismissTimer = setTimeout(() => {
      set({ visible: false });
      _dismissTimer = null;
    }, 2000);
  },
}));
