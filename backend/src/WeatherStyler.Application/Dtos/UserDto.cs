namespace WeatherStyler.Application.Dtos;

public record UserDto(
    Guid Id,
    string Username,
    string Email
);
