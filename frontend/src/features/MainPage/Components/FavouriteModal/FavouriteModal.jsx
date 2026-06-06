import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { ModalFooter } from "../../../../common/components/ModalFooter/ModalFooter";
import { useFavouriteOutfitStore } from "../../../Favourites/favouriteStore";
import "./FavouriteModal.scss";
import { AiOutlineHeart } from "react-icons/ai";
import { useState } from "react";
import { alreadyExists, isTextValid} from "../../../../utils/helpers";

export function FavouriteModal({clothes, outfitId, onClose}) {

    const {addFavouriteOutfit} = useFavouriteOutfitStore();
    const [outfitName, setOutfitName] = useState("");
    const [outfitDescription, setOutfitDescription] = useState("");

    async function Save(){
        if (!outfitName.trim()) return;

        if(!isTextValid(outfitName, 3, 50)) {
            alert("Nazwa outfitu musi mieć od 3 do 50 znaków");
            return;
        }

        if (alreadyExists(useFavouriteOutfitStore.getState().favouriteOutfits, outfitName)) {
            alert("Outfit o takiej nazwie już istnieje w ulubionych");
            return;
        }

        if(outfitDescription && !isTextValid(outfitDescription, 0, 150)){
            alert("Opis outfitu może mieć maksymalnie 150 znaków");
            return;
        }

        await addFavouriteOutfit({
            id: outfitId, 
            name: outfitName, 
            description: outfitDescription,
            isFavourite: true, 
        });
        onClose();
    }
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
                        <input type="text" className="description-text" placeholder="Outfit #1..." value={outfitName} onChange={(e)=> setOutfitName(e.target.value)}></input>
                    </div>
                    <div className="form-description">
                        <span className="description-title">Opis outfitu</span>
                        <input type="text" className="description-text" placeholder="Krótki opis outfitu..." value={outfitDescription} onChange={(e)=> setOutfitDescription(e.target.value)}></input>
                    </div>
                    <ModalFooter onClose={onClose} onSave={Save}/>
                </div>
            </div>
        </div>
    </>
  );
}
