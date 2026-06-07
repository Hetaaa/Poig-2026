import "./Clothingitem.scss";
import { backendBaseUrl } from "../../../api/apiClient";

export function ClothingItem({ name, categoryName, photoUrl, className = "" }) {
  const fullPhotoUrl = photoUrl
    ? photoUrl.startsWith("http")
      ? photoUrl
      : `${backendBaseUrl}${photoUrl}`
    : null;
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
