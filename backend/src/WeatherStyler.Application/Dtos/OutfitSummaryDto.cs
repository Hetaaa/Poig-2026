using System;
using System.Collections.Generic;

namespace WeatherStyler.Domain.Contracts;

public record OutfitSummaryDto(
    Guid UsageHistoryId,
    DateTime DateWorn,
    int Rating,
    bool IsFavourite,
    Guid OutfitId,
    string OutfitName,
    int ItemCount
);

public record OutfitGenerationResultDto(
    IEnumerable<OutfitSummaryDto>? Summaries,
    List<string>? Warnings
);
