import { useState, useEffect } from "react";
import "./FavouriteOutfitCard.scss";
import { AiOutlineHeart } from "react-icons/ai";

export default function FavouriteOutfitCard(){
    return (
    <>
        <div className="Favourite-container">
            <div className="Outfit-header">
                <div className="header-title">
                    <div className="favourite-outfit-title">
                        <span className="favourite-outfit-text">Outfit #1 </span>
                    </div>
                    <span className="header-description">Bardzo wygodny na zimniejsze letnie dni</span>
                </div>
                <div className="Outfit-date">
                    <span className="date-text">Zapisano 15 Marca 2026</span>
                    <AiOutlineHeart className="medium-icon color"/>
                </div>
            </div>
            <div className="favourite-clothes">
                <div className="favourite-item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                        <p className="item-title">Czarna MISBHV</p>
                        <p className="item-layer">Warstwa średnia</p>
                        <p className="item-name">Bluza</p>
                    </div>
                </div>
                <div className="favourite-item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                       <p className="item-title">Biała koszulka</p>
                        <p className="item-layer">Warstwa bazowa</p>
                        <p className="item-name">Koszulka</p>
                    </div>
                </div>
                <div className="favourite-item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                        <p className="item-title">Jeansy niebieskie</p>
                        <p className="item-layer">Dół</p>
                        <p className="item-name">Spodnie</p>
                    </div>
                </div>
                <div className="favourite-item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                        <p className="item-title">Nike</p>
                        <p className="item-layer">Obuwie</p>
                        <p className="item-name">buty</p>
                    </div>
                </div>
            </div>
        </div>
    </>
    )
}