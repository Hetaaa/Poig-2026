import { useState } from "react";
import "./Outfit.scss";
import { BiShuffle } from "react-icons/bi";
import { AiOutlineHeart} from "react-icons/ai";
import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { FavouriteModal } from "../FavouriteModal/FavouriteModal";

const clothes = [
  { id: 1, name: "Czarna MISBHV", layer: "Warstwa średnia", category: "Bluza" },
  { id: 2, name: "Biała koszulka", layer: "Warstwa bazowa", category: "Koszulka" },
  { id: 3, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie" },
];

export function Outfit() {
    const[showFavouriteModal, setShowFavouriteModal] = useState(false);
    return (
    <>
        <div className="outfit-container">
            <div className="header">
                <div className="title">
                    <div className= "title-header">
                        <span className="title-description">Twój outfit na dzis</span>
                    </div>
                    <span className="description">96% Dopasowania • Idealny na dzisiejszą pogodę</span>
                </div>
                <div className="react-img">
                    <button className="icon-btn" aria-label="Shuffle outfit">
                        <BiShuffle className="medium-icon" />
                    </button>

                    <button className="icon-btn" aria-label="Add outfit to favorites" onClick={()=> setShowFavouriteModal(true)}>
                        <AiOutlineHeart className="medium-icon heart"/>
                    </button>
                </div>
           </div>

            <div className="clothes">
                {clothes.map((item) => (
                <ClothingItem
                    key={item.id}
                    name={item.name}
                    layer={item.layer}
                    category={item.category}
                />
            ))}
      </div>

           {showFavouriteModal && <FavouriteModal clothes={clothes} onClose={()=> setShowFavouriteModal(false)}/>}
        </div>
    </>
  );
}
