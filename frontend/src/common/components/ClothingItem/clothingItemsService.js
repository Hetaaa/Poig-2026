import { apiClient } from "../../../api/apiClient";

export async function getClothingItems(){
    const response = await apiClient.get("/ClothingItems");
    return response.data;
}

export async function createClothingItem(item) {
    const response = await apiClient.post("/ClothingItems", item);
    return response.data;
}