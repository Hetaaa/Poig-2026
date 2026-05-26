import { useState, useEffect } from "react";
import "./WardrobeTitle.scss"
import { AiOutlinePlus } from "react-icons/ai";
import { NavLink } from "react-router-dom";


export function WardrobeTitle(){

  return (
    <>
      <div className="Wardrobe-Header">
        <div className="Wardrobe-text">
            <span className="text-title">Moja garderoba</span>
            <span className="text-description">Zarządzaj swoimi ubraniami</span>
        </div>
        <div className="space"></div>
        <NavLink to="/add-clothing" className="Button-add">
            <span className="button-text">Dodaj nowe ubranie</span>
            <AiOutlinePlus className="button-icon"/>
        </NavLink>
      </div>
    </>
  );

}