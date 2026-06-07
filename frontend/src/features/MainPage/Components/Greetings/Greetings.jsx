import { useState, useEffect } from "react";
import { formatFullDate, formatTime } from "../../../../utils/dateUtil";
import "./Greetings.scss";

function getGreetings(date) { 
  const hour = date.getHours();
  if (hour >= 5 && hour < 12) return "Dzień dobry";
  if (hour >= 12 && hour < 18) return "Miłego popołudnia";
  if (hour >= 18 && hour < 22) return "Dobry wieczór";

  return "Dobranoc";
}

export function Greetings() {
  const [currentDate, setCurrentDate] = useState(new Date());

  useEffect(() => {
    const intervalId = setInterval(() => {
      setCurrentDate(new Date());
    }, 1000);
    return () => clearInterval(intervalId);
  }, []);


const formattedGreetings = getGreetings(currentDate);
const formattedDate = formatFullDate(currentDate);
const formattedTime = formatTime(currentDate);

  return (
    <>
      <div className="greetings">
        <div className="greetings-content">
          <span className="title">{formattedGreetings}</span>
          <span className="date">{formattedDate}</span>
        </div>
        <div className="greetings-time">
          <div className="time">{formattedTime}</div>
        </div>
      </div>
    </>
  );
}
