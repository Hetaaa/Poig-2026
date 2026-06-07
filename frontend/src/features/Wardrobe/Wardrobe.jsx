import React, { useEffect } from "react";
import "./Wardrobe.scss";
import { AiOutlinePlus } from "react-icons/ai";
import { ClothingSection } from "./Components/ClothingSection/ClothingSection";
import { useAddElementStore } from "../AddClothing/addElementStore";
import { useClothingItemsStore } from "../../common/components/ClothingItem/clothingItemsStore";

export function Wardrobe() {
  const { openAdd } = useAddElementStore();
  const { clothingItems, status, error, fetchClothingItems } =
    useClothingItemsStore();

  useEffect(() => {
    fetchClothingItems();
  }, [fetchClothingItems]);

  return (
    <>
      <div className="wardrobe-header">
        <div className="wardrobe-text">
          <span className="text-title">Moja garderoba</span>
          <span className="text-description">Zarządzaj swoimi ubraniami</span>
        </div>
        <button onClick={openAdd} className="button-add">
          <span className="button-text">Dodaj nowe ubranie</span>
          <AiOutlinePlus className="button-icon" />
        </button>
      </div>
      {status === "loading" && <p>Ładowanie ubrań...</p>}
      {status === "error" && <p>Błąd: {error} </p>}
      {status === "success" && clothingItems.length === 0 && (
        <p>Nie masz jeszcze żadnych ubrań w szafie</p>
      )}
      {status === "success" && clothingItems.length > 0 && <ClothingSection />}
    </>
  );
}
