import React from "react";
import "./Wardrobe.scss";
import { NavLink } from "react-router-dom";
import { AiOutlinePlus } from "react-icons/ai";
import {ClothingSection} from "./Components/ClothingSection/ClothingSection";
import { useAddElementStore } from "../AddClothing/addElementStore";

export function Wardrobe() {
  const {openAdd} = useAddElementStore();
  return (
    <>
      <div className="wardrobe-header">
        <div className="wardrobe-text">
            <span className="text-title">Moja garderoba</span>
            <span className="text-description">Zarządzaj swoimi ubraniami</span>
        </div>
        <div className="space"></div>
        <button onClick = {openAdd} className="button-add">
            <span className="button-text">Dodaj nowe ubranie</span>
            <AiOutlinePlus className="button-icon"/>
        </button>
      </div>

      <ClothingSection/>
    </>
  );
}
