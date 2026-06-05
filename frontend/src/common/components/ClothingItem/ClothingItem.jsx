import "./ClothingItem.scss";

export function ClothingItem({name, layer, category, className = ""}) {
  return (
    <div className={`clothing-item ${className}`}>
      <div className="medium-component"></div>

      <div className="clothing-item-text">
        <p className="clothing-item-name">{name}</p>
        <p className="clothing-item-layer">{layer}</p>
        <p className="clothing-item-category">{category}</p>
      </div>
    </div>
  );
}