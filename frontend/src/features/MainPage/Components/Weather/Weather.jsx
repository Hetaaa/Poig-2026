import { useEffect } from "react";
import "./Weather.scss";
import { useWeatherStore } from "./weatherStore";
import { AiOutlineCloud, AiOutlineEye, AiOutlineSun } from "react-icons/ai";
import { CiTempHigh } from "react-icons/ci";
import { IoWaterOutline, IoRainyOutline } from "react-icons/io5";
import { BiWind } from "react-icons/bi";
import { TiWeatherWindyCloudy } from "react-icons/ti";


function WeatherDetail ({icon: Icon, label, value}){
  return (
    <div className="weather-detail">
      <div className="frame">
        <Icon className="small-icon" />
        <div className="weather-text">
          <span className="label">{label}</span>
          <span className="value">{value}</span>
        </div>
      </div>
    </div>
  );
}

export function Weather() {

  const weatherStatus = "windy";

  const weatherIcons = {
    cloudy: AiOutlineCloud, 
    sunny: AiOutlineSun, 
    raining: IoRainyOutline, 
    windy: TiWeatherWindyCloudy,
  };

  const weatherDescriptions = {
    cloudy: "Zachmurzenie", 
    sunny: "Słonecznie", 
    raining: "Deszczowo", 
    windy: "Wietrznie",
  };

  const WeatherIcon = weatherIcons[weatherStatus] || AiOutlineCloud
  const weatherDescription = weatherDescriptions[weatherStatus] || "Zachmurzenie"

  const { weather, status, error, fetchDailyWeather, setLastLocation } =
    useWeatherStore();

  // TODO: Change to actual data when discussed how
  const warsawLocation = {
    latitude: 52.2297,
    longitude: 21.0122,
  };

  useEffect(() => {
    const loadWeather = async () => {
      await setLastLocation(warsawLocation);
      await fetchDailyWeather();
    };
    loadWeather();
  }, [setLastLocation, fetchDailyWeather]);

  useEffect(() => {
    console.log("Pogoda:", weather);
  }, [weather]);

  return (
      <div className={`weather-block weather-${weatherStatus}`}>
        <div className="info">
          <div className="city-info">
            <div className="city-text">
              <AiOutlineCloud className="small-icon" />
              <span className="city">Warszawa, Polska</span>
            </div>
            <div className="city-temperature">
              <h1 className="temp-big">18°C</h1>
              <span className="temp-small">/ 15°C</span>
            </div>
            <p className="opis">{weatherDescription}</p>
          </div>
          <WeatherIcon className="big-icon" />
        </div>
        <div className="line"></div>
        <div className="weather-info">
          <WeatherDetail icon = {CiTempHigh} label="Odczuwalna" value = "16°C"/>
          <WeatherDetail icon = {IoWaterOutline} label="Wilgotność" value = "67%"/>
          <WeatherDetail icon = {BiWind} label="Wiatr" value = "12km"/>
          <WeatherDetail icon= {AiOutlineEye} label="Widoczność" value="10km"/>
        </div>
      </div>
  );
}