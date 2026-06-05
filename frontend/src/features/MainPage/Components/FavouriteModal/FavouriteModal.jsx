import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { ModalFooter } from "../../../../common/components/ModalFooter/ModalFooter";
import "./FavouriteModal.scss";
import { AiOutlineHeart } from "react-icons/ai";


export function FavouriteModal({clothes, onClose}) {
    return (
    <>
        <div className="favourite-preview">
            <div className="favourite-card">
                <div className="card-header">
                    <div className="header">
                        <AiOutlineHeart className="mini-icon color"/>
                        <span className="header-text">Dodawanie do ulubionych</span>
                    </div>
                </div>
                <div className="card-outfit">
                    {clothes.map((item)=> (<ClothingItem key={item.id} name={item.name} layer={item.layer} category={item.category}/>))}
                </div>
                <div className="card-form">
                    <div className="form-description">
                        <span className="description-title">Nazwa outfitu</span>
                        <input type="text" className="description-text" placeholder="Outfit #1..."></input>
                    </div>
                    <div className="form-description">
                        <span className="description-title">Opis outfitu</span>
                        <input type="text" className="description-text" placeholder="Krótki opis outfitu..."></input>
                    </div>
                    <ModalFooter onClose={onClose}/>
                </div>
            </div>
        </div>
    </>
  );
}
