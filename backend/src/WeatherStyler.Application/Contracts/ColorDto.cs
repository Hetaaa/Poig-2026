namespace WeatherStyler.Application.Contracts;

public record ColorDto(
    Guid Id, 
    string Name, 
    bool IsNeutral  // True for neutral colors (black, white, gray, beige)
);
