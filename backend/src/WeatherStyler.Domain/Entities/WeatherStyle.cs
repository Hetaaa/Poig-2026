namespace WeatherStyler.Domain.Entities;

public class WeatherStyle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = "#3B82F6";
}
