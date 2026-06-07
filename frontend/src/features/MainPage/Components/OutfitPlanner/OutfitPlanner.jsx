import { useEffect } from "react";
import { useState } from "react";
import { OutfitModal } from "../OutfitModal/OutfitModal";
import "./OutfitPlanner.scss";
import { BiLinkExternal } from "react-icons/bi";
import { useOutfitStore } from "../Outfit/outfitStore";

function OutfitCard({ date, outfit }) {
  const [showOutfitModal, setShowOutfitModal] = useState(false);
  const items = outfit?.clothingItems ?? [];

  const formattedDate = date
    ? new Date(date).toLocaleDateString("pl-PL", { weekday: "long", day: "numeric", month: "short" })
    : outfit?.name ?? "";

  return (
    <div className="outfit-card">
      <div className="description">
        <div className="description-text">
          <span className="description-date">{formattedDate}</span>
          <span className="description-weather">{items.length} elementów</span>
        </div>
        <button
          type="button"
          className="icon-btn"
          aria-label="Open outfit details"
          onClick={() => setShowOutfitModal(true)}
        >
          <BiLinkExternal className="medium-icon color" />
        </button>
      </div>
      <div className="outfit-item">
        {items.map((item) => (
          <div key={item.id} className="small-component"></div>
        ))}
      </div>

      <span className="item-count">{items.length} elementów</span>

      {showOutfitModal && (
        <OutfitModal
          date={date}
          clothes={items}
          onClose={() => setShowOutfitModal(false)}
        />
      )}
    </div>
  );
}

export function OutfitPlanner() {
  const { upcomingOutfits, upcomingStatus, fetchUpcomingOutfits } =
    useOutfitStore();

  useEffect(() => {
    fetchUpcomingOutfits(3);
  }, [fetchUpcomingOutfits]);

  if (upcomingStatus === "loading") return <p>Ładowanie propozycji...</p>;
  if (upcomingStatus === "error") return <p>Nie udało się pobrać propozycji</p>;
  if (upcomingStatus === "success" && upcomingOutfits.length === 0) {
    return <p>Brak zapisanych outfitów na najbliższe dni</p>;
  }

  return (
    <div className="outfit-plan">
      {upcomingOutfits.map((entry, index) => (
        <OutfitCard key={entry.outfit?.id ?? index} date={entry.date} outfit={entry.outfit} />
      ))}
    </div>
  );
}
