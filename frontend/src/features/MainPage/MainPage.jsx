import React from "react";
import "./MainPage.scss";
import { BiCalendarAlt } from "react-icons/bi";
import { Greetings } from "./Components/Greetings/Greetings";
import { Weather } from "./Components/Weather/Weather";
import  {Outfit}  from "./Components/Outfit/Outfit";
import {OutfitPlanner}  from "./Components/OutfitPlanner/OutfitPlanner";

export function MainPage() {
  return (
    <>
      <Greetings />
      <Weather />
      <Outfit/>

      <div className="planner-title">
        <span className="title">Propozycje na najbliższe dni</span>
        <BiCalendarAlt className="medium-icon"/>
      </div>

      <OutfitPlanner/>
    </>
  );
}
