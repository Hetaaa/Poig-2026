import { useEffect } from "react";
import "./Weather.scss";
import { useWeatherStore } from "./weatherStore";
import { useLocationStore } from "../../../../common/stores/locationStore";
import { AiOutlineCloud, AiOutlineSun } from "react-icons/ai";
import { CiTempHigh } from "react-icons/ci";
import { IoWaterOutline, IoRainyOutline } from "react-icons/io5";
import { BiWind } from "react-icons/bi";
import { TiWeatherWindyCloudy } from "react-icons/ti";
import {
  getWeatherStatus,
  weatherDescriptions,
} from "../../../../utils/weatherHelpers";

function WeatherDetail({ icon: Icon, label, value }) {
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
  };

  const { weather, status, error, fetchDailyWeather } = useWeatherStore();
  const { location } = useLocationStore();

  useEffect(() => {
    fetchDailyWeather();
  }, [fetchDailyWeather]);

  if (status === "loading") {
    return (
      <div className="weather-block weather-cloudy">
        <div className="info">
          <div className="city-info">
            <div className="city-text">
              <AiOutlineCloud className="small-icon" />
              <span className="city">Ładowanie...</span>
            </div>
            <div className="city-temperature">
              <h1 className="temp-big">--°C</h1>
              <span className="temp-small">/--°C</span>
            </div>
            <p className="opis">Pobieranie danych pogodowych</p>
          </div>
          <AiOutlineCloud className="big-icon" />
        </div>
      </div>
    );
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
  const WeatherIcon = weatherIcons[weatherStatus];
  const weatherDescription = weatherDescriptions[weatherStatus];

  return (
    <div className={`weather-block weather-${weatherStatus}`}>
      <div className="info">
        <div className="city-info">
          <div className="city-text">
            <AiOutlineCloud className="small-icon" />
            <span className="city">Moja lokalizacja</span>
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
        <WeatherDetail
          icon={CiTempHigh}
          label="Odczuwalna"
          value={`${feelsLike}°C`}
        />
        <WeatherDetail
          icon={IoWaterOutline}
          label="Wilgotność"
          value={`${humidity}%`}
        />
        <WeatherDetail
          icon={BiWind}
          label="Wiatr"
          value={`${windSpeed} km/h`}
        />
        <WeatherDetail
          icon={AiOutlineCloud}
          label="Zachmurzenie"
          value={`${cloudCover}%`}
        />
      </div>
    </div>
  );
}
