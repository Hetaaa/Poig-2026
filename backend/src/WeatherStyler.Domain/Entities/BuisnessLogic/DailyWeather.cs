namespace WeatherStyler.Domain.Entities.BuisnessLogic;

public record DailyWeather(
    DateTime Date,
    double Temperature,
    double FeelsLike,
    double Humidity,
    double WindSpeed,
    double Precipitation,
    double CloudCover
);
