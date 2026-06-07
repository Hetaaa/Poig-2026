import { apiClient } from "../../../../api/apiClient";
import { formatDate } from "../../../../utils/dateUtil";

export async function getTodayOutfit() {
  const response = await apiClient.get("/Outfit/today");
  return response.data;
}

export async function getOutfitsByRange(from, to) {
  const fromStr = formatDate(from);
  const toStr = formatDate(to);
  const response = await apiClient.get("/Outfit", {
    params: { from: fromStr, to: toStr },
  });
  return response.data;
}

export async function regenerateTodayOutfit() {
  const response = await apiClient.post("/Outfit/today/regenerate");
  return response.data;
}
