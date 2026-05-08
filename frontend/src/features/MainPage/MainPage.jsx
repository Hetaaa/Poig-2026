import React from "react";
import "./MainPage.scss";
import { Greetings } from "./Components/Greetings/Greetings";
import { Weather } from "./Components/Weather/Weather";

export function MainPage() {
  return (
    <>
      <Greetings />
      <Weather />
    </>
  );
}
