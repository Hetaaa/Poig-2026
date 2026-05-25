import { useState, useEffect } from "react";
import "./Outfit.scss";
import { BiShuffle } from "react-icons/bi";
import { AiOutlineHeart } from "react-icons/ai";

export default function Outfit() {
    return (
    <>
        <div className="Outfit-container">
            <div className="Header">
                <div className="Title">
                    <div className= "title-header">
                        <span className="title-description">Twój outfit na dzis</span>
                    </div>
                    <span className="description">96% Dopasowania • Idealny na dzisiejszą pogodę</span>
                </div>
                <div className="React-img">
                    <BiShuffle className="medium-icon" />
                    <AiOutlineHeart className="medium-icon"/>
                </div>
           </div>
           <div className="clothes">
                <div className="item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                        <p className="item-title">Czarna MISBHV</p>
                        <p className="item-layer">Warstwa średnia</p>
                        <p className="item-name">Bluza</p>
                    </div>
                </div>
                <div className="item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                        <p className="item-title">Biała koszulka</p>
                        <p className="item-layer">Warstwa bazowa</p>
                        <p className="item-name">Koszulka</p>
                    </div>
                </div> 
                <div className="item">
                    <div className="medium-component">
                    </div>
                    <div className="item-text">
                        <p className="item-title">Jeansy niebieskie</p>
                        <p className="item-layer">Dół</p>
                        <p className="item-name">Spodnie</p>
                    </div>
                </div>
                <div className="item">
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
  );
}
