namespace WeatherStyler.Application.Contracts;

public record CategoryDto(
    Guid Id, 
    string Name, 
    int LayerIndex  // 1=Base, 2=Middle, 3=Outer
);
