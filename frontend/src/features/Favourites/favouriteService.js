import { apiClient } from "../../api/apiClient";

export async function getFavouriteOutfits() {
  const response = await apiClient.get("/Outfit/favourite");
  return response.data;
}
