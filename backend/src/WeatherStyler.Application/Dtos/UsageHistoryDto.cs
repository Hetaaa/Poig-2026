namespace WeatherStyler.Application.Dtos;

public record UsageHistoryDto(
    Guid Id,
    DateTime DateWorn,
    int Rating
);
