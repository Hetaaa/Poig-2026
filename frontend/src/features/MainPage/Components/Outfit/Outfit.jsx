import { useState, useEffect } from "react";
import "./Outfit.scss";
import { BiShuffle } from "react-icons/bi";
import { AiOutlineHeart, AiFillHeart } from "react-icons/ai";
import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import { useOutfitStore } from "./outfitStore";
import { apiClient } from "../../../../api/apiClient";
import { useFavouriteOutfitStore } from "../../../Favourites/favouriteStore";
import { useAlertStore } from "../../../../common/components/AlertBox/alertStore";

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
  const {
    todayOutfit,
    todayWarnings,
    todayStatus,
    fetchTodayOutfit,
    regenerateTodayOutfit,
  } = useOutfitStore();
  const categoryMap = useCategoryMap();
  const { favouriteOutfits, fetchFavouriteOutfits, changeFavourite } =
    useFavouriteOutfitStore();
  const { showAlert } = useAlertStore();

  useEffect(() => {
    fetchTodayOutfit();
    fetchFavouriteOutfits();
  }, [fetchTodayOutfit, fetchFavouriteOutfits]);

  const isCurrentOutfitFav = favouriteOutfits.some(
    (fav) => fav.id === todayOutfit?.id,
  );

  async function handleFavourite() {
    const wasFav = isCurrentOutfitFav;
    const outfit = {
      id: todayOutfit.id,
      clothes: items,
    };

    try {
      await changeFavourite(outfit);
      if (!wasFav) {
        showAlert("Dodano outfit do ulubionych");
      } else {
        showAlert("Usunięto outfit z ulubionych");
      }
    } catch {
      showAlert("Nie udało się zmienić ulubionych. Spróbuj ponownie.", "error");
    }
  }

  async function handleRegenerate() {
    if (isCurrentOutfitFav) {
      showAlert(
        "Ten outfit jest w ulubionych. Usuń go z ulubionych, aby wylosować nowy.",
        "error",
      );
      return;
    }

    await regenerateTodayOutfit();
  }

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
              {todayStatus === "loading"
                ? "Ładowanie..."
                : (todayOutfit?.name ?? "")}
            </span>
          </div>
          <div className="react-img">
            <button
              className="icon-btn"
              aria-label="Shuffle outfit"
              onClick={handleRegenerate}
              disabled={todayStatus === "loading"}
            >
              <BiShuffle className="medium-icon" />
            </button>

            <button
              className="icon-btn"
              aria-label="Add outfit to favorites"
              onClick={handleFavourite}
            >
              <span className="heart-icon">
                <AiOutlineHeart className="medium-icon heart" />
                <AiFillHeart
                  className={`medium-icon heart heart-fill${isCurrentOutfitFav ? " heart-fill--active" : ""}`}
                />
              </span>
            </button>
          </div>
        </div>

        {todayStatus === "loading" && <p>Generowanie outfitu...</p>}
        {todayStatus === "error" && (
          <p>
            {todayWarnings?.length > 0
              ? todayWarnings[0]
              : "Nie udało się pobrać outfitu na dziś"}
          </p>
        )}
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
      </div>
    </>
  );
}
