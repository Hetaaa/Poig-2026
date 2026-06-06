export function isNight(){
  const hour = new Date().getHours();
  return hour > 20 || hour < 6;
}

export function getWeatherStatus(weather) {
  if (!weather) return "cloudy";

  const night = isNight();

  if (weather.windSpeed > 25) return night ? "night-windy": "windy";
  if (weather.precipitation > 0.5) return night ? "night-raining": "raining";
  if (weather.cloudCover > 70) return night ? "night-cloudy":"cloudy";

  return night ? "night-clear": "sunny";
}

export const weatherDescriptions = {
  cloudy: "Zachmurzenie",
  sunny: "Słonecznie",
  raining: "Deszczowo",
  windy: "Wietrznie",

  "night-clear": "Bezchmurna noc",
  "night-cloudy": "Pochmurna noc",
  "night-raining": "Deszczowa noc",
  "night-windy": "Wietrzna noc",

};