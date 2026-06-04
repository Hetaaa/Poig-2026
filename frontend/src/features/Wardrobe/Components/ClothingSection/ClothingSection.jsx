import { useState, useEffect } from "react";
import "./ClothingSection.scss";
import { BiSolidHot } from "react-icons/bi";
import { BiEdit } from "react-icons/bi";
import { RiDeleteBin6Line } from "react-icons/ri";

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
    return (
    <>
    <div className="out-layer">
        <h3 className="out-text">Warstwa średnia</h3>
        <div className="out-card">
            <OuterLayer title = "Kurtka sztruks" category="Kurtka" warmth={8}/>
        </div>
    </div>
    
    <div className="line"></div>

    <div className="out-layer">
        <h3 className="out-text">Warstwa średnia</h3>
        <div className="out-card">
            <OuterLayer title = "Czarna MISBHV" category="Bluza" warmth={7}/>
            <OuterLayer title = "Czarna MISBHV" category="Bluza" warmth={7}/>
            <OuterLayer title = "Czarna MISBHV" category="Bluza" warmth={7}/>
        </div>
    </div>

    <div className="line"></div>

    </>
    )
}