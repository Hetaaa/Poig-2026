import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { ModalFooter } from "../../../../common/components/ModalFooter/ModalFooter";
import "./FavouriteModal.scss";
import { AiOutlineHeart } from "react-icons/ai";
import { useState } from "react";
import { useFavouriteOutfitStore } from "../../../Favourites/favouriteStore";
import { alreadyExists, isTextValid} from "../../../../utils/helpers";
import { useAlertStore } from "../../../../common/components/AlertBox/alertStore";

export function FavouriteModal({clothes, outfitId, onClose}) {

    const {changeFavourite} = useFavouriteOutfitStore();
    const { showAlert } = useAlertStore();

    const [saving, setSaving] = useState(false);

    async function Save(){
        setSaving(true);

        try{
            console.log("outfitId w modalu:", outfitId);
            console.log("clothes w modalu:", clothes);
            const outfit = {
                id: outfitId,
                clothes, 
            };

            await changeFavourite(outfit);

            onClose();
        } catch {
            showAlert("Nie udało się dodać outfitu do ulubionych.", "error");
        } finally {
            setSaving(false);
        }
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
                    {clothes?.map((item)=> (<ClothingItem key={item.id} name={item.name} category={item.categoryId}/>))}
                </div>
                <div className="card-form">
                    <ModalFooter onClose={onClose} onSave={Save} disabled={saving}/>
                </div>
            </div>
        </div>
    </>
  );
}
