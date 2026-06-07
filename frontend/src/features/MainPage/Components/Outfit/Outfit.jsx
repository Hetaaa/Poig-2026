import { useState, useEffect } from "react";
import "./Outfit.scss";
import { BiShuffle } from "react-icons/bi";
import { AiOutlineHeart } from "react-icons/ai";
import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { FavouriteModal } from "../FavouriteModal/FavouriteModal";
import { useOutfitStore } from "./outfitStore";
import { apiClient } from "../../../../api/apiClient";

function useCategoryMap() {
  const [categoryMap, setCategoryMap] = useState({});
  useEffect(() => {
    apiClient
      .get("/Lookup/categories")
      .then((res) => {
        const map = {};
        res.data.forEach((cat) => {
          map[cat.id] = cat.name;
        });
        setCategoryMap(map);
      })
      .catch(() => {});
  }, []);
  return categoryMap;
}

export function Outfit() {
  const [showFavouriteModal, setShowFavouriteModal] = useState(false);

  const { todayOutfit, todayStatus, fetchTodayOutfit, regenerateTodayOutfit } = useOutfitStore();
  const categoryMap = useCategoryMap();

  useEffect(() => {
    fetchTodayOutfit();
  }, [fetchTodayOutfit]);

  const items = todayOutfit?.clothingItems ?? [];

  return (
    <>
      <div className="outfit-container">
        <div className="header">
          <div className="title">
            <div className="title-header">
              <span className="title-description">Twój outfit na dziś</span>
            </div>
            <span className="description">
              {todayOutfit ? todayOutfit.name : "Ładowanie..."}
            </span>
          </div>
          <div className="react-img">
            <button className="icon-btn" aria-label="Shuffle outfit" onClick={regenerateTodayOutfit} disabled={todayStatus === "loading"}>
              <BiShuffle className="medium-icon" />
            </button>

            <button
              className="icon-btn"
              aria-label="Add outfit to favorites"
              onClick={() => setShowFavouriteModal(true)}
            >
              <AiOutlineHeart className="medium-icon heart" />
            </button>
          </div>
        </div>

        {todayStatus === "loading" && <p>Generowanie outfitu...</p>}
        {todayStatus === "error" && <p>Nie udało się pobrać outfitu na dziś</p>}
        {todayStatus === "success" && items.length === 0 && (
          <p>Brak outfitu – dodaj ubrania do garderoby</p>
        )}

        <div className="clothes">
          {items.map((item) => (
            <ClothingItem
              key={item.id}
              name={item.name}
              categoryName={categoryMap[item.categoryId] ?? ""}
              photoUrl={item.photoUrl}
            />
          ))}
        </div>

        {showFavouriteModal && (
          <FavouriteModal
            clothes={items}
            onClose={() => setShowFavouriteModal(false)}
          />
        )}
      </div>
    </>
  );
}
