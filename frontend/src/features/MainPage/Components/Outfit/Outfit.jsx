import { useState, useEffect } from "react";
import "./Outfit.scss";
import { BiShuffle } from "react-icons/bi";
import { AiOutlineHeart, AiFillHeart } from "react-icons/ai";
import { ClothingItem } from "../../../../common/components/ClothingItem";

export function Outfit() {
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

                    <button className="icon-btn" aria-label="Add outfit to favorites">
                        <AiOutlineHeart className="medium-icon heart"/>
                        <AiFillHeart className="medium-icon heart"/>
                    </button>
                </div>
           </div>

           <div className="clothes">
            <ClothingItem name="Czarna MISBHV" layer="Warstwa średnia" category="Bluza" />
            <ClothingItem name="Biała koszulka" layer="Warstwa bazowa" category="Koszulka" />
            <ClothingItem name="Jeansy niebieskie" layer="Dół" category="Spodnie" />
            <ClothingItem name="Nike" layer="Obuwie" category="Buty" />
           </div>
        </div>
    </>
  );
}
