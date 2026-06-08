import { apiClient } from "../../api/apiClient";

export async function getFavouriteOutfits() {
  const response = await apiClient.get("/Outfit/favourite");
  return response.data;
}

export async function changeFavouriteStatus(outfitId, isFavourite){
  const response = await apiClient.put(`/Outfit/${outfitId}/favourite`, 
    null, 
    {
      params: {
        isFavourite,
      },
    }
  );

  return response.data;
}
