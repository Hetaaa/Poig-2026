import { ClothingItem } from "../../../../common/components/ClothingItem/ClothingItem";
import "./OutfitModal.scss";
import { AiOutlineClose } from "react-icons/ai";
import { useState, useEffect } from "react";
import { apiClient } from "../../../../api/apiClient";
import { formatDate } from "../../../../utils/dateUtil";

function useCategoryMap() {
  const [categoryMap, setCategoryMap] = useState({});
  useEffect(() => {
    apiClient
      .get("/Lookup/categories")
      .then((res) => {
        const map = {};
        res.data.forEach((cat) => {
          map[cat.id] = cat.name;
        });
        setCategoryMap(map);
      })
      .catch(() => {});
  }, []);
  return categoryMap;
}

function useDayWeather(date) {
  const [temp, setTemp] = useState(null);

  useEffect(() => {
    if (!date) return;
    const dateStr = formatDate(new Date(date));
    apiClient
      .get(`/WeatherPicker/daily/${dateStr}`)
      .then((res) => setTemp(res.data?.temperature?.toFixed(0) ?? null))
      .catch(() => {});
  }, [date]);

  return temp;
}

export function OutfitModal({ date, clothes, onClose }) {
  const categoryMap = useCategoryMap();
  const temp = useDayWeather(date);

  const formattedDate = date
    ? new Date(date).toLocaleDateString("pl-PL", { weekday: "long", day: "numeric", month: "long" })
    : "";

  return (
    <>
      <div className="outfit-preview">
        <div className="preview-card">
          <div className="preview-header">
            <div className="header-text">
              <span className="text-title">{formattedDate}</span>
              <span className="text-details">
                {temp !== null ? `${temp}°C` : "-- °C"}
              </span>
            </div>
            <button
              type="button"
              className="header-icon"
              aria-label="Zamknij podgląd outfitu"
              onClick={onClose}
            >
              <AiOutlineClose />
            </button>
          </div>
          <div className="preview-clothes">
            {clothes.map((item) => (
              <ClothingItem
                key={item.id}
                name={item.name}
                categoryName={categoryMap[item.categoryId] ?? ""}
                photoUrl={item.photoUrl}
              />
            ))}
          </div>
        </div>
      </div>
    </>
  );
}
