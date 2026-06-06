import { useState } from "react";
import { MdOutlineShoppingBag } from "react-icons/md";
import { IoIosArrowDown } from "react-icons/io";
import { ModalFooter } from "../../../common/components/ModalFooter/ModalFooter";
import "./AddElement.scss";

function CategoryOptions() {
  return (
    <>
      <option value="base">Koszulka (warstwa bazowa)</option>
      <option value="base">Koszula (warstwa bazowa)</option>

      <option value="middle">Bluza (warstwa średnia)</option>
      <option value="middle">Sweter (warstwa średnia)</option>

      <option value="outer">Kurtka (warstwa wierzchnia)</option>

      <option value="bottom">Spodnie (warstwa dolna)</option>
      <option value="bottom">Spodenki (warstwa dolna)</option>
      <option value="bottom">Spódnica (warstwa dolna)</option>

      <option value="shoes">Obuwie</option>
    </>
  );
}

export function AddElement({onClose}) {
  const [photoPreview, setPhotoPreview] = useState(null);

  function PhotoChange(e){
    const file = e.target.files[0];
    
    if(file){
      setPhotoPreview(URL.createObjectURL(file));
    }
  }
  return (
    <div className="add-page">
      <div className="add-card">
        <div className="card-container">
          <div className="card-header">
            <MdOutlineShoppingBag className="card-icon"/>
            <span className="card-title">Dodawanie ubrania</span>
          </div>
        </div>

        <div className="add-clothes">
          <div className="clothes-detail">
            <div className="clothes-image">
              <span className="image-description">Zdjęcie ubrania</span>
              <label className="image-upload">
                <input type="file" accept="image/*" hidden onChange={PhotoChange} />
                {photoPreview ? (<img src={photoPreview} className="uploaded-image"></img>):(
                <span className="upload-icon">+</span>)}
              </label>
            </div>

            <div className="clothes-option">
              <span className="clothes-title">Nazwa ubrania</span>
              <input className="clothes-input"
                type="text"
                placeholder="Czarna bluza..."
              />
            </div>

            <div className="clothes-option">
              <span className="category-title">Kategoria</span>
              <IoIosArrowDown className="select-icon" />
              <select className="category-select">
                <CategoryOptions />
              </select>
            </div>

            <div className="clothes-warmth">
              <div className="warmth-form">
                <span className="warmth-title">Poziom ciepła (1-10)</span>
                <input name = "warmthLevel"
                className="warmth-range" type="range" min="1" max="10"/>
              </div>
            </div>

            <div className="clothes-waterproof">
              <input name="waterproof"
              type="checkbox" className="waterproof-checkbox" />
              <span className="waterproof-text">Wodoodporne</span>
            </div>
          </div>
          <ModalFooter onClose={onClose}/>
        </div>
      </div>
    </div>
  );
}