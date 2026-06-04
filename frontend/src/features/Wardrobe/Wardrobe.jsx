import React from "react";
import "./Wardrobe.scss";
import { NavLink } from "react-router-dom";
import { AiOutlinePlus } from "react-icons/ai";
import {ClothingSection} from "./Components/ClothingSection/ClothingSection";
export function Wardrobe() {
  return (
    <>
      <div className="wardrobe-header">
        <div className="wardrobe-text">
            <span className="text-title">Moja garderoba</span>
            <span className="text-description">Zarządzaj swoimi ubraniami</span>
        </div>
        <div className="space"></div>
        <NavLink to="/add-clothing" className="Button-add">
            <span className="button-text">Dodaj nowe ubranie</span>
            <AiOutlinePlus className="button-icon"/>
        </NavLink>
      </div>

      <ClothingSection/>
    </>
  );
}
