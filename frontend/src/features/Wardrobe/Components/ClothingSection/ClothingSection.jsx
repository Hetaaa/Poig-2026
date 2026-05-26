import { useState, useEffect } from "react";
import "./ClothingSection.scss";
import { BiSolidHot } from "react-icons/bi";
import { BiEdit } from "react-icons/bi";
import { RiDeleteBin6Line } from "react-icons/ri";


export default function Outerlayer(){
    return (
    <>
    <div className="Out-layer">
        <h3 className="out-text">Warstwa wierzchnia</h3>
        <div className="out-card">
            <div className="out-item">
                <div className="medium-component"></div>
                <div className="wardrobe-text">
                    <span className="wardrobe-title">Kurtka sztruks</span>
                    <div className="wardrobe-description">
                        <span className="description-text">Kurtka</span>
                        <div className="description-rate">
                            <div className="description-rate-text">8/10</div>
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
        </div>
    </div>

    <div className="line"></div>

    <div className="Out-layer">
        <h3 className="out-text">Warstwa średnia</h3>
        <div className="out-card">
            <div className="out-item">
                <div className="medium-component"></div>
                <div className="wardrobe-text">
                    <span className="wardrobe-title">Czarna MISBHV</span>
                    <div className="wardrobe-description">
                        <span className="description-text">Bluza</span>
                        <div className="description-rate">
                            <div className="description-rate-text">7/10</div>
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

            <div className="out-item">
                <div className="medium-component"></div>
                <div className="wardrobe-text">
                    <span className="wardrobe-title">Czarna MISBHV</span>
                    <div className="wardrobe-description">
                        <span className="description-text">Bluza</span>
                        <div className="description-rate">
                            <div className="description-rate-text">7/10</div>
                            <BiSolidHot className="small-icon color"/>
                        </div>
                    </div>
                </div>
                <div className="out-icons">
                    <div className="bck-icon color">
                        <BiEdit  className="mini-icon"/>
                    </div>
                    <div className="bck-icon color1">
                        <RiDeleteBin6Line className="mini-icon color-icon"/>
                    </div>
                </div>
            </div>
            <div className="out-item">
                <div className="medium-component"></div>
                <div className="wardrobe-text">
                    <span className="wardrobe-title">Szara</span>
                    <div className="wardrobe-description">
                        <span className="description-text">Bluza</span>
                        <div className="description-rate">
                            <div className="description-rate-text">7/10</div>
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
        </div>
    </div>

    <div className="line"></div>

    </>
    )
}