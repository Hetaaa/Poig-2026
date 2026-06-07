import { apiClient } from "../../../api/apiClient";

export async function getClothingItems() {
  const response = await apiClient.get("/ClothingItems");
  return response.data;
}

export async function createClothingItem(item) {
  const formData = new FormData();
  formData.append("Name", item.name);
  formData.append("CategoryId", item.categoryId);
  formData.append("WarmthLevel", item.warmthLevel);
  if (item.photoFile) {
    formData.append("PhotoFile", item.photoFile);
  }
  if (item.colorIds && item.colorIds.length > 0) {
    item.colorIds.forEach((id) => formData.append("ColorIds", id));
  }
  if (item.styleIds && item.styleIds.length > 0) {
    item.styleIds.forEach((id) => formData.append("StyleIds", id));
  }
  if (item.properties && item.properties.length > 0) {
    item.properties.forEach((prop, i) => {
      formData.append(`Properties[${i}].Name`, prop.name);
      formData.append(`Properties[${i}].Value`, prop.value);
    });
  }

  const response = await apiClient.post("/ClothingItems", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  console.log("[createClothingItem] FormData entries:");
  for (const [key, value] of formData.entries()) {
    console.log(`  ${key}:`, value);
  }
  return response.data;
}

export async function updateClothingItem(id, item) {
  await apiClient.put(`/ClothingItems/${id}`, {
    name: item.name,
    categoryId: item.categoryId,
    warmthLevel: item.warmthLevel,
    styleIds: item.styleIds ?? [],
    colorIds: item.colorIds ?? [],
    properties: item.properties ?? [],
  });
}

export async function deleteClothingItem(id) {
  await apiClient.delete(`/ClothingItems/${id}`);
}
