import { useState, useEffect } from "react";
import "./OutfitPlanner.scss";
import { BiLinkExternal } from "react-icons/bi";


export default function OutfitPlanner() {   
    return (
    <>
        <div className="OutfitPlan">
            <div className="OutfitCard">
                <div className="Description">
                    <div className="description-text">
                        <span className="description-date">Wtorek, 8 Kwietnia</span>
                        <span className="description-weather">19°C • Ciepło, bez kurtki</span>
                    </div>
                    <button className="icon-btn">
                        <BiLinkExternal className="medium-icon color" />
                    </button>
                </div>
                <div className="OutfitItem">
                    <div className="small-component"></div>
                    <div className="small-component"></div>
                    <div className="small-component"></div>
                </div>
                <span className="ItemCount">3 elementy</span>
            </div>
            <div className="OutfitCard">
                <div className="Description">
                    <div className="description-text">
                        <span className="description-date">Wtorek, 8 Kwietnia</span>
                        <span className="description-weather">15°C • Chłodniej, potrzebny sweter</span>
                    </div>
                    <button className="icon-btn">
                        <BiLinkExternal className="medium-icon color" />
                    </button>
                </div>
                <div className="OutfitItem">
                    <div className="small-component"></div>
                    <div className="small-component"></div>
                    <div className="small-component"></div>
                </div>
                <span className="ItemCount">4 elementy</span>
            </div>
            <div className="OutfitCard">
                <div className="Description">
                    <div className="description-text">
                        <span className="description-date">Wtorek, 8 Kwietnia</span>
                        <span className="description-weather">11°C • Zimno, kurtka obowiązkowa</span>
                    </div>
                    <button className="icon-btn">
                        <BiLinkExternal className="medium-icon color" />
                    </button>
                </div>
                <div className="OutfitItem">
                    <div className="small-component"></div>
                    <div className="small-component"></div>
                    <div className="small-component"></div>
                </div>
                <span className="ItemCount">5 elementów</span>
            </div>
        </div>
    </>
  );
}
