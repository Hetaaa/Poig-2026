using WeatherStyler.Domain.Entities.BuisnessLogic;
using System.Collections.Generic;

namespace WeatherStyler.Domain.Interfaces.Services;

using WeatherStyler.Domain.Entities.BuisnessLogic;

public interface IOutfitManagerService
{
    Task<OutfitGeneratorResult> GenerateOutfitForTodayAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OutfitGeneratorResult> GetOrGenerateTodayAsync(Guid userId, CancellationToken cancellationToken = default);
}
