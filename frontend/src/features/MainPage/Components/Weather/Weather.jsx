import { useEffect } from "react";
import "./Weather.scss";
import { useWeatherStore } from "./weatherStore";
import { AiOutlineCloud, AiOutlineSun } from "react-icons/ai";
import { CiTempHigh } from "react-icons/ci";
import { BsMoonStars } from "react-icons/bs";
import { IoWaterOutline, IoRainyOutline } from "react-icons/io5";
import { BiWind } from "react-icons/bi";
import { TiWeatherWindyCloudy } from "react-icons/ti";
import { getWeatherStatus, weatherDescriptions } from "../../../../utils/weatherHelpers";


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

  const weatherIcons = {
    cloudy: AiOutlineCloud, 
    sunny: AiOutlineSun, 
    raining: IoRainyOutline, 
    windy: TiWeatherWindyCloudy,

    "night-clear": BsMoonStars,
    "night-cloudy": AiOutlineCloud,
    "night-raining": IoRainyOutline,
    "night-windy": TiWeatherWindyCloudy,
  };


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

  if (status === "loading") {
    return <p>Ładowanie pogody...</p>;
  }

  if (status === "error") {
    return <p>{error}</p>;
  }

  if (!weather) {
    return null;
  }

  const temperature = weather?.temperature?.toFixed(0) ?? "-";
  const feelsLike = weather?.feelsLike?.toFixed(0) ?? "-";
  const humidity = weather?.humidity?.toFixed(0) ?? "-";
  const windSpeed = weather?.windSpeed?.toFixed(0) ?? "-";
  const cloudCover = weather?.cloudCover?.toFixed(0) ?? "-";

 
  const weatherStatus = getWeatherStatus(weather);
  const WeatherIcon = weatherIcons[weatherStatus]
  const weatherDescription = weatherDescriptions[weatherStatus]
  
  return (
      <div className={`weather-block weather-${weatherStatus}`}>
        <div className="info">
          <div className="city-info">
            <div className="city-text">
              <AiOutlineCloud className="small-icon" />
              <span className="city">Warszawa, Polska</span>
            </div>
            <div className="city-temperature">
              <h1 className="temp-big">{temperature}°C</h1>
              <span className="temp-small">/{feelsLike}°C</span>
            </div>
            <p className="opis">{weatherDescription}</p>
          </div>
          <WeatherIcon className="big-icon" />
        </div>
        <div className="line"></div>
        <div className="weather-info">
          <WeatherDetail icon = {CiTempHigh} label="Odczuwalna" value = {`${feelsLike}°C`}/>
          <WeatherDetail icon = {IoWaterOutline} label="Wilgotność" value = {`${humidity}%`}/>
          <WeatherDetail icon = {BiWind} label="Wiatr" value = {`${windSpeed} km/h`}/>
          <WeatherDetail icon= {AiOutlineCloud} label="Zachmurzenie" value={`${cloudCover}%`}/>
        </div>
      </div>
  );
}