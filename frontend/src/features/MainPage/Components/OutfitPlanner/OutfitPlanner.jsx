import { useState, useEffect } from "react";
import { OutfitModal } from "../OutfitModal/OutfitModal";
import "./OutfitPlanner.scss";
import { BiLinkExternal } from "react-icons/bi";

const clothes = [
  { id: 1, name: "Czarna MISBHV", layer: "Warstwa średnia", category: "Bluza" },
  { id: 2, name: "Biała koszulka", layer: "Warstwa bazowa", category: "Koszulka" },
  { id: 3, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie" },
  { id: 4, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie" },
  { id: 5, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie" },
  { id: 6, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie" },
  { id: 7, name: "Jeansy niebieskie", layer: "Dół", category: "Spodnie" },
];

function OutfitCard({date, weather, description, itemCount, clothes}) {

  const[showOutfitModal, setShowOutfitModal] = useState(false);

  return (
    <div className="outfit-card">
        <div className="description">
            <div className="description-text">
                <span className="description-date">{date}</span>
                <span className="description-weather">{weather}°C • {description}</span>
            </div>
            <button type="button" className="icon-btn" aria-label="Open outfit details" onClick={()=> setShowOutfitModal(true)}>
                <BiLinkExternal className="medium-icon color" />
            </button>
        </div>
        <div className="outfit-item">
            {clothes.map((item)=> (
              <div key={item.id} className="small-component"></div>
            ))}
        </div>

        <span className="item-count">{itemCount}</span>

        {showOutfitModal && (
        <OutfitModal date={date} weather={weather} description={description} clothes={clothes} onClose={() => setShowOutfitModal(false)} />)}
    </div>
  );
}

export function OutfitPlanner() {   
    return (
    <div className="outfit-plan">
      <OutfitCard
        date="Wtorek, 8 Kwietnia"
        weather="19"
        description="Ciepło, bez kurtki"
        itemCount="3 elementy"
        clothes={clothes}
      />

      <OutfitCard
        date="Wtorek, 8 Kwietnia"
        weather="15"
        description="Chłodniej, potrzebny sweter"
        itemCount="4 elementy"
        clothes={clothes}
      />

      <OutfitCard
        date="Wtorek, 8 Kwietnia"
        weather="11"
        description="Zimno, kurtka obowiązkowa"
        itemCount="5 elementów"
        clothes={clothes}
      />
    </div>
  );
}
