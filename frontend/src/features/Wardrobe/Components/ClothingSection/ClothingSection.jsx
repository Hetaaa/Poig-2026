import "./ClothingSection.scss";
import { BiSolidHot } from "react-icons/bi";
import { BiEdit } from "react-icons/bi";
import { RiDeleteBin6Line } from "react-icons/ri";
import { useClothingItemsStore } from "../../../../common/components/ClothingItem/clothingItemsStore";

function getLayerByCategory(category) {
    if (["Koszulka", "Koszula"].includes(category)) return "base";
    if (["Bluza", "Sweter"].includes(category)) return "middle";
    if (["Spodnie", "Spódnica"].includes(category)) return "bottom";
    if (["Kurtka"].includes(category)) return "outer";
    if (["Obuwie"].includes(category)) return "shoes";

    return "other";
}

function mappingItem(items){
    if (!Array.isArray(items)) return null;

    return items.map((item)=>(
        <OuterLayer
            key = {item.id}
            title = {item.name}
            category = {item.category?.name}
            warmth = {item.warmthLevel}
        />
    ));
}

function layer(title, items) {
    if (!items.length) return null;

    return (
        <>
            <div className="out-layer">
                <h3 className="out-text">{title}</h3>
                <div className="out-card">{mappingItem(items)}</div>
            </div>
            
            <div className="line"></div>
        </>
    )
}

function OuterLayer ({title, category, warmth}) {
    return (
        <div className="out-item">
            <div className="medium-component"></div>
            <div className="wardrobe-text">
                <span className="wardrobe-title">{title}</span>
                <div className="wardrobe-description">
                    <span className="description-text">{category}</span>
                    <div className="description-rate">
                        <div className="description-rate-text">{warmth}/10</div>
                        <BiSolidHot className="small-icon color"/>
                    </div>
                </div>
            </div>
        <div className="out-icons">
            <div className="bck-icon color">
            <button className="icon-btn edit" aria-label="Edytuj ubranie">
                <BiEdit className="mini-icon"/>
            </button>
            </div>
            <div className="bck-icon color1">
                <button className="icon-btn" aria-label="Usuń ubranie">
                    <RiDeleteBin6Line className="mini-icon color-icon"/>
                </button>
            </div>
        </div>
    </div>
    );
}


export function ClothingSection(){
    const {clothingItems} = useClothingItemsStore();


    const items = Array.isArray(clothingItems) ? clothingItems : [];

    const baseLayer = items.filter(
        (item) => getLayerByCategory(item.category?.name) === "base"
    );

    const middleLayer = items.filter(
        (item) => getLayerByCategory(item.category?.name) === "middle"
    );

    const outerLayer = items.filter(
        (item) => getLayerByCategory(item.category?.name) === "outer"
    );

    const bottomLayer = items.filter(
        (item) => getLayerByCategory(item.category?.name) === "bottom"
    );

    const shoeLayer = items.filter(
        (item) => getLayerByCategory(item.category?.name) === "shoes"
    );

    return (
    <>
        {layer("Warstwa bazowa", baseLayer)}
        {layer("Warstwa średnia", middleLayer)}
        {layer("Warstwa dolna", bottomLayer)}
        {layer("Warstwa wierzchnia", outerLayer)}
        {layer("Obuwie", shoeLayer)}
    </>
    )
}