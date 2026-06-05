import { useState, useEffect } from "react";
import { formatFullDate, formatTime } from "../../../../utils/dateUtil";
import "./Greetings.scss";

export function Greetings() {
  const [currentDate, setCurrentDate] = useState(new Date());

  useEffect(() => {
    const intervalId = setInterval(() => {
      setCurrentDate(new Date());
    }, 1000);
    return () => clearInterval(intervalId);
  }, []);

const formattedDate = formatFullDate(currentDate);
const formattedTime = formatTime(currentDate);

  return (
    <>
      <div className="greetings">
        <div className="greetings-content">
          <span className="title">Dzień dobry!</span>
          <span className="date">{formattedDate}</span>
        </div>
        <div className="greetings-time">
          <div className="time">{formattedTime}</div>
        </div>
      </div>
    </>
  );
}
