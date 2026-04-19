using WeatherStyler.Domain.Common;
namespace WeatherStyler.Domain.Entities.BuisnessLogic;

public record WeatherDataForGeneration(
    int Temperature,        // Temperatura w °C
    bool IsRaining,        // Czy pada deszcz?
    bool IsWindy,          // Czy jest wietrznie?
    bool IsSunny           // Czy jest słonecznie?
);