export function getWeatherStatus(weather) {
  if (!weather) return "cloudy";

  if (weather.windSpeed > 25) return "windy";
  if (weather.precipitation > 0.5) return "raining";
  if (weather.cloudCover > 70) return "cloudy";

  return "sunny";
}

export const weatherDescriptions = {
  cloudy: "Zachmurzenie",
  sunny: "Słonecznie",
  raining: "Deszczowo",
  windy: "Wietrznie",
};