import { useState, useEffect } from "react";
import "./FavouriteOutfitCard.scss";
import { AiFillHeart} from "react-icons/ai";
import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { useFavouriteOutfitStore } from "../../favouriteStore"; 

export function FavouriteOutfitCard(){
    const {favouriteOutfits, fetchFavouriteOutfits, changeFavourite, status, error} = useFavouriteOutfitStore();

    useEffect(()=>{
        fetchFavouriteOutfits();
    }, [fetchFavouriteOutfits]);

    if(status==="loading") return <p>Ładowanie ulubionych outfitów...</p>;
    if(error) return <p>{error}</p>;

return (
  <>
    {favouriteOutfits.length === 0 && (
      <p>Brak ulubionych outfitów.</p>
    )}

    {favouriteOutfits.map((outfit, index) => (
      <div className="favourite-container" key={outfit.id || index}>
        <div className="outfit-header">
          <div className="header-title">
            <span className="favourite-outfit-text">
              {`Outfit #${index + 1}`}
            </span>
          </div>

          <div className="outfit-date">
            <span className="date-text">Zapisano w ulubionych</span>

            <button className="icon-btn" aria-label="Usuń z ulubionych" onClick = {()=> changeFavourite(outfit)}>
              <AiFillHeart className="medium-icon color" />
            </button>
          </div>
        </div>

        <div className="favourite-clothes">
          {(outfit.items || outfit.clothingItems || outfit.clothes || []).map((item) => (
            <ClothingItem
              key={item.id}
              name={item.name}
              category={item.categoryId}
            />
          ))}
        </div>
      </div>
    ))}
  </>
);
}