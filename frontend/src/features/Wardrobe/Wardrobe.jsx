import React from "react";
import "./Wardrobe.scss";
import ClothingSection from "./Components/ClothingSection/ClothingSection";
import { WardrobeTitle } from "./Components/Headline/WardrobeTitle";

export function Wardrobe() {
  return (
    <>
      <WardrobeTitle/>
      <ClothingSection/>
    </>
  );
}
