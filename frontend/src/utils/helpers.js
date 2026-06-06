export function getErrorMessage(error) {
  if (typeof error === "string") return error;
  return (
    error?.response?.data?.message ||
    error?.message ||
    "Nie udalo sie pobrac danych"
  );
}

export function isTextValid(text, min=3, max=50){
  const trimmedText = text.trim();
  return trimmedText.length >= min && trimmedText.length <=max;
}

export function alreadyExists(items = [], name = "") {
  return items.some(
    (item) =>
      item.name?.trim().toLowerCase() === name.trim().toLowerCase()
  );
}