import "./ClothingSection.scss";
import { BiSolidHot } from "react-icons/bi";
import { BiEdit } from "react-icons/bi";
import { RiDeleteBin6Line } from "react-icons/ri";
import { useClothingItemsStore } from "../../../../common/components/ClothingItem/clothingItemsStore";
import { useEffect, useState } from "react";
import { apiClient, backendBaseUrl } from "../../../../api/apiClient";
import { EditElement } from "../../../AddClothing/Components/EditElement";

const SLOT_LABELS = {
  Core: "Torso",
  Nogi: "Spodnie",
  Głowa: "Nakrycie głowy",
  Stopy: "Obuwie",
};

const SLOT_ORDER = ["Głowa", "Core", "Nogi", "Stopy"];

function useCategoryData() {
  const [categoryMap, setCategoryMap] = useState({});
  const [categorySlots, setCategorySlots] = useState({});

  useEffect(() => {
    apiClient
      .get("/Lookup/categories")
      .then((res) => {
        const nameMap = {};
        const slotsMap = {};
        res.data.forEach((cat) => {
          nameMap[cat.id] = cat.name;
          slotsMap[cat.id] = (cat.clothingSlots ?? []).map((s) => s.name);
        });
        setCategoryMap(nameMap);
        setCategorySlots(slotsMap);
      })
      .catch(() => {});
  }, []);

  return { categoryMap, categorySlots };
}

function OuterLayer({
  id,
  title,
  categoryName,
  warmth,
  photoUrl,
  itemData,
  onDelete,
  onEdit,
}) {
  const fullPhotoUrl = photoUrl
    ? photoUrl.startsWith("http")
      ? photoUrl
      : `${backendBaseUrl}${photoUrl}`
    : null;
  const bgStyle = fullPhotoUrl
    ? { backgroundImage: `url(${fullPhotoUrl})` }
    : {};

  return (
    <div className="out-item">
      <div className="medium-component" style={bgStyle}></div>
      <div className="wardrobe-text">
        <span className="wardrobe-title">{title}</span>
        <div className="wardrobe-description">
          <span className="description-text">{categoryName}</span>
          <div className="description-rate">
            <div className="description-rate-text">{warmth}/10</div>
            <BiSolidHot className="small-icon color" />
          </div>
        </div>
      </div>
      <div className="out-icons">
        <div
          className="bck-icon color1"
          onClick={() => onDelete(id)}
          aria-label="Usuń ubranie"
        >
          <button className="icon-btn">
            <RiDeleteBin6Line className="mini-icon color-icon" />
          </button>
        </div>
      </div>
    </div>
  );
}

function SlotSection({ slotName, items, onDelete, onEdit, categoryMap }) {
  if (!items.length) return null;
  const label = SLOT_LABELS[slotName] ?? slotName;

  return (
    <div className="slot-section">
      <h3 className="slot-title">{label}</h3>
      <div className="slot-scroll">
        {items.map((item) => (
          <OuterLayer
            key={item.id}
            id={item.id}
            title={item.name}
            categoryName={categoryMap[item.categoryId] ?? ""}
            warmth={item.warmthLevel}
            photoUrl={item.photoUrl}
            itemData={item}
            onDelete={onDelete}
            onEdit={onEdit}
          />
        ))}
      </div>
    </div>
  );
}

export function ClothingSection() {
  const { clothingItems, removeClothingItem } = useClothingItemsStore();
  const { categoryMap, categorySlots } = useCategoryData();
  const [editItem, setEditItem] = useState(null);

  const items = Array.isArray(clothingItems) ? clothingItems : [];

  async function remove(id) {
    await removeClothingItem(id);
  }

  const grouped = {};
  SLOT_ORDER.forEach((slot) => (grouped[slot] = []));

  items.forEach((item) => {
    const slots = categorySlots[item.categoryId] ?? [];
    const matchedSlot = SLOT_ORDER.find((slot) => slots.includes(slot));
    if (matchedSlot) {
      grouped[matchedSlot].push(item);
    }
  });

  return (
    <>
      <div className="clothing-sections">
        {SLOT_ORDER.map((slot) => (
          <SlotSection
            key={slot}
            slotName={slot}
            items={grouped[slot]}
            onDelete={remove}
            onEdit={setEditItem}
            categoryMap={categoryMap}
          />
        ))}
      </div>
      {editItem && (
        <EditElement item={editItem} onClose={() => setEditItem(null)} />
      )}
    </>
  );
}
