import "./ClothingSection.scss";
import { BiSolidHot } from "react-icons/bi";
import { BiEdit } from "react-icons/bi";
import { RiDeleteBin6Line } from "react-icons/ri";
import { useClothingItemsStore } from "../../../../common/components/ClothingItem/clothingItemsStore";

function mappingItem(items, onDelete){
    if (!Array.isArray(items)) return null;

    return items.map((item)=>(
        <OuterLayer
            key = {item.id}
            id = {item.id}
            title = {item.name}
            categoryId = {item.categoryId}
            warmth = {item.warmthLevel}
            onDelete = {onDelete}
        />
    ));
}

function layer(title, items, onDelete) {
    if (!items.length) return null;

    return (
        <>
            <div className="out-layer">
                <h3 className="out-text">{title}</h3>
                <div className="out-card">{mappingItem(items, onDelete)}</div>
            </div>
            
            <div className="line"></div>
        </>
    )
}

function OuterLayer ({id, title, categoryId, warmth, onDelete}) {
    return (
        <div className="out-item">
            <div className="medium-component"></div>
            <div className="wardrobe-text">
                <span className="wardrobe-title">{title}</span>
                <div className="wardrobe-description">
                    <span className="description-text">{categoryId}</span>
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
                <button className="icon-btn" aria-label="Usuń ubranie" onClick={()=> onDelete(id)}>
                    <RiDeleteBin6Line className="mini-icon color-icon"/>
                </button>
            </div>
        </div>
    </div>
    );
}


export function ClothingSection(){
    const {clothingItems, removeClothingItem} = useClothingItemsStore();

    const items = Array.isArray(clothingItems) ? clothingItems : [];

    async function remove(id){
        await removeClothingItem(id)
    }

    return (
    <>
        {layer("Ubrania", items, remove)}
    </>
    )
}