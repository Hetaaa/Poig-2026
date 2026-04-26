namespace WeatherStyler.Application.Dtos;

public record ProgramVariableDto(
    Guid Id,
    string Key,
    string Value,
    Guid UserId
);
