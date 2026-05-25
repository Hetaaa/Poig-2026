import { useEffect } from "react";
import "./Weather.scss";
import { useWeatherStore } from "./weatherStore";
import { AiOutlineCloud, AiOutlineEye } from "react-icons/ai";
import { CiTempHigh } from "react-icons/ci";
import { IoWaterOutline } from "react-icons/io5";
import { BiWind } from "react-icons/bi";




export function Weather() {
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
    <>
      <div className="Weather-block">
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
            <p className="opis">Zachmurzenie</p>
          </div>
          <AiOutlineCloud className="big-icon" />
        </div>
        <div className="line"></div>
        <div className="weather-info">
          <div className="weather-detail">
            <div className="frame">
              <CiTempHigh className="small-icon" />
              <div className="weather-text">
                <span className="label">Odczuwalna</span>
                <span className="value">16°C</span>
              </div>
            </div>
          </div>
          <div className="weather-detail">
            <div className="frame">
              <IoWaterOutline className="small-icon"/>
              <div className="weather-text">
                <span className="label">Wilgotność</span>
                <span className="value">67%</span>
              </div>
            </div>
          </div>
          <div className="weather-detail">
            <div className="frame">
              <BiWind className="small-icon" />
              <div className="weather-text">
                <span className="label">Wiatr</span>
                <span className="value">12 km/h</span>
              </div>
            </div>
          </div>
          <div className="weather-detail">
            <div className="frame">
              <AiOutlineEye className="small-icon" />
              <div className="weather-text">
                <span className="label">Widoczność</span>
                <span className="value">10 km</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
