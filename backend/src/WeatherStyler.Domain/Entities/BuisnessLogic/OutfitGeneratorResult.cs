using WeatherStyler.Domain.Entities;

public class OutfitGeneratorResult
{
    public Outfit? Outfit { get; set; }
    public List<string> Warnings { get; set; } = new();

    // Additional debug info when outfit generation fails
    public double? Temperature { get; set; }
    public bool? IsWindy { get; set; }
    public bool? IsSunny { get; set; }
    public bool? IsRaining { get; set; }
}