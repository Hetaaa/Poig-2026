import { useState, useEffect } from "react";
import "./Greetings.scss";


export default function Greetings() {
    const [currentDate, setCurrentDate] = useState(new Date())
    
    useEffect(() => {
        const intervalId = setInterval(()=> {
            setCurrentDate(new Date());
        }, 1000);
        return () => clearInterval(intervalId);
    }, []);

    const formattedDate = currentDate.toLocaleDateString('pl-PL', {
        weekday: 'long', 
        day: 'numeric', 
        month: 'long', 
        year: 'numeric'
    })

    const formattedTime = currentDate.toLocaleTimeString('pl-PL', {
        hour: '2-digit',
        minute: '2-digit'
    })

    return (
    <>
        <div className="Greetings">
            <div className="Greetings_content">
                <h1 className="title">Dzień dobry!</h1>
                <p className="date">{formattedDate}</p>
            </div>
            <div className="time">{formattedTime}</div>
        </div>
    </>
  );
}
