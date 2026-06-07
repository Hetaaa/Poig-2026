export function formatDate(date) {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

export function formatTime(date){
  return date.toLocaleTimeString("pl-PL", {
  hour: "2-digit",
  minute: "2-digit",
});
}

export function formatFullDate(date){
  return date.toLocaleDateString("pl-PL", {
  weekday: "long",
  day: "numeric",
  month: "long",
  year: "numeric",
});
}
