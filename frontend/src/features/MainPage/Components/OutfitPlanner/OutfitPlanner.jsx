import { useState, useEffect } from "react";
import "./OutfitPlanner.scss";
import { BiLinkExternal } from "react-icons/bi";

function OutfitCard({date, weather, description, itemCount}) {
  return (
    <div className="outfit-card">
        <div className="description">
            <div className="description-text">
                <span className="description-date">{date}</span>
                <span className="description-weather">{weather}°C • {description}</span>
            </div>
            <button className="icon-btn" aria-label="Open outfit details">
                <BiLinkExternal className="medium-icon color" />
            </button>
        </div>
        <div className="outfit-item">
            <div className="small-component"></div>
            <div className="small-component"></div>
            <div className="small-component"></div>
        </div>
        <span className="item-count">{itemCount}</span>
    </div>
  );
}

export function OutfitPlanner() {   
    return (
    <div className="outfit-plan">
      <OutfitCard
        date="Wtorek, 8 Kwietnia"
        weather="19°C • Ciepło, bez kurtki"
        itemCount="3 elementy"
      />

      <OutfitCard
        date="Wtorek, 8 Kwietnia"
        weather="15°C • Chłodniej, potrzebny sweter"
        itemCount="4 elementy"
      />

      <OutfitCard
        date="Wtorek, 8 Kwietnia"
        weather="11°C • Zimno, kurtka obowiązkowa"
        itemCount="5 elementów"
      />
    </div>
  );
}
