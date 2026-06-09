import "./Clothingitem.scss";
import { backendBaseUrl } from "../../../api/apiClient";
// 1. Importujemy oficjalny konwerter ścieżek z rdzenia Tauri v2
import { convertFileSrc } from "@tauri-apps/api/core";

export function ClothingItem({ name, categoryName, photoUrl, className = "" }) {
  // 2. Budujemy pełny adres URL do backendu .NET
  const rawPhotoUrl = photoUrl
    ? photoUrl.startsWith("http")
      ? photoUrl
      : `${backendBaseUrl}${photoUrl}`
    : null;

  // 3. Konwertujemy adres URL na bezpieczny protokół Tauri (odporny na blokady CSP)
  const fullPhotoUrl = rawPhotoUrl ? convertFileSrc(rawPhotoUrl) : null;

  const bgStyle = fullPhotoUrl
    ? { backgroundImage: `url(${fullPhotoUrl})` }
    : {};

  return (
    <div className={`clothing-item ${className}`}>
      <div className="medium-component" style={bgStyle}></div>

      <div className="clothing-item-text">
        <p className="clothing-item-name">{name}</p>
        <p className="clothing-item-category">{categoryName}</p>
      </div>
    </div>
  );
}