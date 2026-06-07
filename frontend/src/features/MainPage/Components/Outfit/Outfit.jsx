import { useState, useEffect } from "react";
import "./Outfit.scss";
import { BiShuffle } from "react-icons/bi";
import { AiOutlineHeart} from "react-icons/ai";
import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { FavouriteModal } from "../FavouriteModal/FavouriteModal";
import { useWeatherStore } from "../Weather/weatherStore";
import { useClothingItemsStore } from "../../../../common/components/ClothingItem/clothingItemsStore";

const clothes = [
  { id: 1, name: "Czarna MISBHV", layer: "Warstwa średnia", category: "Bluza", warmth: 3, waterproof: false},
  { id: 2, name: "Biała koszulka", layer: "Warstwa bazowa", category: "Koszulka", warmth: 1, waterproof: false},
  { id: 3, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie", warmth: 2, waterproof: false},
];

export function Outfit() {
    const[showFavouriteModal, setShowFavouriteModal] = useState(false);
    const[currentOutfit, setCurrentOutfit] = useState([])

    const {weather} = useWeatherStore();
    const {clothingItems, fetchClothingItems} = useClothingItemsStore();
    
    useEffect(() => {
    fetchClothingItems();
    }, [fetchClothingItems]);

    return (
    <>
        <div className="outfit-container">
            <div className="header">
                <div className="title">
                    <div className= "title-header">
                        <span className="title-description">Twój outfit na dziś</span>
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
                {currentOutfit.map((item) => (
                <ClothingItem
                    key={item.id}
                    name={item.name}
                    category={item.categoryId}
                />
            ))}
      </div>

           {showFavouriteModal && <FavouriteModal clothes={currentOutfit} onClose={()=> setShowFavouriteModal(false)}/>}
        </div>
    </>
  );
}
