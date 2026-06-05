import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import "./OutfitModal.scss";
import { AiOutlineClose } from "react-icons/ai";


export function OutfitModal({date, weather, description, clothes, onClose}) {
    return (
    <>
        <div className="outfit-preview">
            <div className="preview-card">
                <div className="preview-header">
                    <div className="header-text">
                        <span className="text-title">{date}</span>
                        <span className="text-details">{weather}°C • {description}</span>
                    </div>
                    <button
                      type="button"
                      className="header-icon"
                      aria-label="Zamknij podgląd outfitu"
                      onClick={onClose}
                    >
                        <AiOutlineClose/>
                    </button>
                </div>
                <div className="preview-clothes">
                    {clothes.map((item)=> (<ClothingItem key={item.id} name={item.name} layer={item.layer} category={item.category}/>))}
                </div>
            </div>
        </div>
    </>
  );
}
