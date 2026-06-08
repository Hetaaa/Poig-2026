import { useState, useEffect } from "react";
import { MdOutlineShoppingBag } from "react-icons/md";
import { IoIosArrowDown } from "react-icons/io";
import { ModalFooter } from "../../../common/components/ModalFooter/ModalFooter";
import { isTextValid, alreadyExists } from "../../../utils/helpers";
import "./AddElement.scss";
import { useClothingItemsStore } from "../../../common/components/ClothingItem/clothingItemsStore";
import { apiClient } from "../../../api/apiClient";
import { useAlertStore } from "../../../common/components/AlertBox/alertStore";

export function AddElement({ onClose, clothingItems = [] }) {
  const { addClothingItem } = useClothingItemsStore();
  const { showAlert } = useAlertStore();

  const [photoPreview, setPhotoPreview] = useState(null);
  const [photoFile, setPhotoFile] = useState(null);
  const [warmthLevel, setWarmthLevel] = useState(5);
  const [name, setName] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [categories, setCategories] = useState([]);
  const [colors, setColors] = useState([]);
  const [styles, setStyles] = useState([]);
  const [selectedColorIds, setSelectedColorIds] = useState([]);
  const [selectedStyleIds, setSelectedStyleIds] = useState([]);
  const [waterproof, setWaterproof] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    apiClient
      .get("/Lookup/categories")
      .then((res) => {
        setCategories(res.data);
        if (res.data.length > 0) setCategoryId(res.data[0].id);
      })
      .catch(() => {});
    apiClient
      .get("/Lookup/colors")
      .then((res) => setColors(res.data))
      .catch(() => {});
    apiClient
      .get("/Lookup/styles")
      .then((res) => setStyles(res.data))
      .catch(() => {});
  }, []);

  async function Save() {
    if (!isTextValid(name, 3, 50)) {
      showAlert("Nazwa ubrania musi mieć od 3 do 50 znaków", "error");
      return;
    }

    if (alreadyExists(clothingItems, name)) {
      showAlert("Takie ubranie już istnieje", "error");
      return;
    }

    if (!categoryId) {
      showAlert("Wybierz kategorię ubrania", "error");
      return;
    }

    if (selectedColorIds.length === 0) {
      showAlert("Wybierz co najmniej jeden kolor", "error");
      return;
    }

    setSaving(true);
    try {
      const properties = waterproof
        ? [
            { name: "waterproof", value: "true" },
            { name: "windproof", value: "true" },
          ]
        : [];

      const payload = {
        name,
        categoryId,
        warmthLevel,
        photoFile,
        properties,
        colorIds: selectedColorIds,
        styleIds: selectedStyleIds,
      };
      console.log("[Save] payload:", payload);

      await addClothingItem(payload);
      onClose();
    } catch {
      showAlert("Nie udało się zapisać ubrania. Spróbuj ponownie.", "error");
    } finally {
      setSaving(false);
    }
  }

  function PhotoChange(e) {
    const file = e.target.files[0];
    if (file) {
      setPhotoPreview(URL.createObjectURL(file));
      setPhotoFile(file);
    }
  }

  return (
    <div className="add-page">
      <div className="add-card">
        <div className="card-container">
          <div className="card-header">
            <MdOutlineShoppingBag className="card-icon" />
            <span className="card-title">Dodawanie ubrania</span>
          </div>
        </div>

        <div className="add-clothes">
          <div className="clothes-detail">
            <div className="clothes-image">
              <span className="image-description">Zdjęcie ubrania</span>
              <label className="image-upload">
                <input
                  type="file"
                  accept="image/*"
                  hidden
                  onChange={PhotoChange}
                />
                {photoPreview ? (
                  <img
                    src={photoPreview}
                    alt="Podgląd ubrania"
                    className="uploaded-image"
                  />
                ) : (
                  <span className="upload-icon">+</span>
                )}
              </label>
            </div>

            <div className="clothes-option">
              <span className="clothes-title">Nazwa ubrania</span>
              <input
                className="clothes-input"
                type="text"
                placeholder="Czarna bluza..."
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </div>

            <div className="clothes-option">
              <span className="category-title">Kategoria</span>
              <IoIosArrowDown className="select-icon" />
              <select
                className="category-select"
                value={categoryId}
                onChange={(e) => setCategoryId(e.target.value)}
              >
                {categories.map((cat) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="clothes-pills-option">
              <span className="clothes-title">Kolor</span>
              <div className="pills-group">
                {colors.map((color) => {
                  const selected = selectedColorIds.includes(color.id);
                  return (
                    <button
                      key={color.id}
                      type="button"
                      className={`pill ${selected ? "pill--selected" : ""}`}
                      onClick={() =>
                        setSelectedColorIds((prev) =>
                          selected
                            ? prev.filter((id) => id !== color.id)
                            : [...prev, color.id],
                        )
                      }
                    >
                      {color.name}
                    </button>
                  );
                })}
              </div>
            </div>

            <div className="clothes-pills-option">
              <span className="clothes-title">Styl</span>
              <div className="pills-group">
                {styles.map((style) => {
                  const selected = selectedStyleIds.includes(style.id);
                  return (
                    <button
                      key={style.id}
                      type="button"
                      className={`pill ${selected ? "pill--selected" : ""}`}
                      onClick={() =>
                        setSelectedStyleIds((prev) =>
                          selected
                            ? prev.filter((id) => id !== style.id)
                            : [...prev, style.id],
                        )
                      }
                    >
                      {style.name}
                    </button>
                  );
                })}
              </div>
            </div>

            <div className="clothes-warmth">
              <div className="warmth-form">
                <span className="warmth-title">
                  Poziom ciepła: {warmthLevel}/10
                </span>
                <input
                  name="warmthLevel"
                  className="warmth-range"
                  type="range"
                  min="1"
                  max="10"
                  value={warmthLevel}
                  onChange={(e) => setWarmthLevel(Number(e.target.value))}
                />
              </div>
            </div>

            <div className="clothes-waterproof">
              <input
                name="waterproof"
                type="checkbox"
                className="waterproof-checkbox"
                checked={waterproof}
                onChange={(e) => setWaterproof(e.target.checked)}
              />
              <span className="waterproof-text">
                Wodoodporne i wiatroszczelne
              </span>
            </div>
          </div>
          <ModalFooter onClose={onClose} onSave={Save} disabled={saving} />
        </div>
      </div>
    </div>
  );
}
