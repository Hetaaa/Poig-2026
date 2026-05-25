import { useState, useEffect } from "react";
import "./PlannerTitle.scss";
import { BiCalendarAlt } from "react-icons/bi";



export default function PlannerTitle() {
    const [currentDate, setCurrentDate] = useState(new Date())
    return (
    <>
        <div className="Planner-Title">
            <span className="title">Propozycje na najbliższe dni</span>
            <BiCalendarAlt className="medium-icon"/>
        </div>
    </>
  );
}
