import React from "react";
import "./MainPage.scss";
import { Greetings } from "./Components/Greetings/Greetings";
import { Weather } from "./Components/Weather/Weather";
import  Outfit  from "./Components/Outfit/Outfit";
import OutfitPlanner  from "./Components/OutfitPlanner/OutfitPlanner";
import PlannerTitle  from "./Components/PlannerTitle/PlannerTitle";

export function MainPage() {
  return (
    <>
      <Greetings />
      <Weather />
      <Outfit/>
      <PlannerTitle/>
      <OutfitPlanner/>
    </>
  );
}
