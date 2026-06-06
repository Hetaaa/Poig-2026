export function getErrorMessage(error) {
  if (typeof error === "string") return error;
  return (
    error?.response?.data?.message ||
    error?.message ||
    "Nie udalo sie pobrac pogody"
  );
}