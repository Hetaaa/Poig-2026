import { useState, useEffect } from "react";
import "./FavouriteOutfitCard.scss";
import { AiOutlineHeart, AiFillHeart } from "react-icons/ai";
import {ClothingItem} from "../../../../common/components/ClothingItem";

export function FavouriteOutfitCard(){
    return (
        <div className="favourite-container">
            <div className="outfit-header">
                <div className="header-title">
                    <span className="favourite-outfit-text">Outfit #1 </span>
                    <span className="header-description">Bardzo wygodny na zimniejsze letnie dni</span>
                </div>
                <div className="outfit-date">
                    <span className="date-text">Zapisano 15 Marca 2026</span>
                    <button className="icon-btn" aria-label="Usuń z ulubionych">
                        <AiFillHeart className="medium-icon color"/>
                    </button>
                </div>
            </div>

            <div className="favourite-clothes">
                <ClothingItem name="Czarna MISBHV" layer="Warstwa średnia" category="Bluza" />
                <ClothingItem name="Biała koszulka" layer="Warstwa bazowa" category="Koszulka" />
                <ClothingItem name="Jeansy niebieskie" layer="Dół" category="Spodnie" />
                <ClothingItem name="Nike" layer="Obuwie" category="Buty" />
            </div>
        </div>
    )
}