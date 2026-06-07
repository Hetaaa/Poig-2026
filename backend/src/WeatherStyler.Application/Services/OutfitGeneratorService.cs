using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Entities.BuisnessLogic;
using WeatherStyler.Domain.Interfaces.Repositories;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Application.Services;

internal class SlotRequirement
{
    public string SlotName { get; set; } = string.Empty;
    public List<int> RequiredLayers { get; set; } = new();
    public int MinWarmth { get; set; }
    public int MaxWarmth { get; set; } = 100;
}

internal readonly record struct SlotLayerKey(string SlotName, int Layer);

internal class ScoredCandidate
{
    public ClothingItem Item { get; set; } = null!;
    public double Score { get; set; }
}

public class OutfitManagerService : IOutfitManagerService
{
    private readonly IProgramVariableRepository _programVars;
    private readonly IClothingItemRepository _clothingRepo;
    private readonly ILookupRepository _lookupRepo;
    private readonly IUsageHistoryRepository _usageHistoryRepo;
    private readonly IWeatherService _weatherService;
    private readonly IOutfitRepository _outfitRepo;
    private readonly IConfiguration _configuration;
    private readonly Random _random = new Random();

    private const double ScoreDiversity = 3.0;
    private const double ScoreStyleMatch = 2.0;
    private const double ScoreWarmthIdeal = 2.0;
    private const double ScoreWarmthAccepted = 0.5;
    private const double ScoreWaterproof = 4.0;
    private const double ScoreWindproof = 3.0;
    private const double PenaltyNonNeutralColor = -2.0;

    public OutfitManagerService(
        IProgramVariableRepository programVars,
        IClothingItemRepository clothingRepo,
        ILookupRepository lookupRepo,
        IUsageHistoryRepository usageHistoryRepo,
        IWeatherService weatherService,
        IOutfitRepository outfitRepo,
        IConfiguration configuration)
    {
        _programVars = programVars;
        _clothingRepo = clothingRepo;
        _lookupRepo = lookupRepo;
        _usageHistoryRepo = usageHistoryRepo;
        _weatherService = weatherService;
        _outfitRepo = outfitRepo;
        _configuration = configuration;
    }

    public async Task<OutfitGeneratorResult> GetOrGenerateTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var existingOutfits = (await _outfitRepo.GetOutfitsAsync(userId, today, today.AddDays(1).AddTicks(-1), cancellationToken)).ToList();
        if (existingOutfits.Any())
        {
            return new OutfitGeneratorResult
            {
                Outfit = existingOutfits.First(),
                Warnings = null
            };
        }

        var outfitResult = await GenerateOutfitForTodayAsync(userId, cancellationToken);
        if (outfitResult.Outfit == null)
            return outfitResult;

        if (!await _outfitRepo.UserExistsAsync(userId, cancellationToken))
            return new OutfitGeneratorResult
            {
                Outfit = null,
                Warnings = new List<string> { "Current user not found in database. Cannot save usage history." }
            };

        var itemIds = outfitResult.Outfit.ClothingItems.Select(ci => ci.Id).ToList();
        var outfitId = outfitResult.Outfit.Id == Guid.Empty ? Guid.NewGuid() : outfitResult.Outfit.Id;

        await _outfitRepo.SaveGeneratedOutfitAsync(outfitId, outfitResult.Outfit.Name, outfitResult.Outfit.DateCreated, userId, itemIds, today, cancellationToken);

        return outfitResult;
    }

    public async Task<OutfitGeneratorResult> GetOrGenerateForDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1).AddTicks(-1);

        var existingOutfits = (await _outfitRepo.GetOutfitsAsync(userId, dayStart, dayEnd, cancellationToken)).ToList();
        if (existingOutfits.Any())
            return new OutfitGeneratorResult { Outfit = existingOutfits.First(), Warnings = null };

        var lat = await _programVars.GetValueAsync("last_location_lat", userId, cancellationToken);
        var lon = await _programVars.GetValueAsync("last_location_lon", userId, cancellationToken);

        if (lat is null || lon is null)
            return Failure("Brak zapisanej lokacji użytkownika. Ustaw lokację aby generować outfity.");

        if (!double.TryParse(lat, CultureInfo.InvariantCulture, out double parsedLat) ||
            !double.TryParse(lon, CultureInfo.InvariantCulture, out double parsedLon))
            return Failure("Zapisana lokacja użytkownika ma niepoprawny format numeryczny.");

        var daily = await _weatherService.GetDailySummaryAsync(parsedLat, parsedLon, dayStart, cancellationToken);
        if (daily is null)
            return Failure("Nie udało się pobrać danych pogodowych dla tej daty.");

        var thresholds = _configuration.GetSection("WeatherThresholds").Get<WeatherThresholds>() ?? new WeatherThresholds();

        var weather = new WeatherDataForGeneration(
            Temperature: (int)Math.Round(daily.Temperature),
            IsRaining: daily.Precipitation > thresholds.RainThreshold,
            IsWindy: daily.WindSpeed > thresholds.WindThreshold,
            IsSunny: daily.CloudCover < thresholds.SunnyCloudThreshold
        );

        var outfitResult = await GenerateOutfitWithWeatherAsync(userId, weather, cancellationToken);
        if (outfitResult.Outfit == null)
            return outfitResult;

        if (!await _outfitRepo.UserExistsAsync(userId, cancellationToken))
            return Failure("Current user not found in database. Cannot save usage history.");

        var itemIds = outfitResult.Outfit.ClothingItems.Select(ci => ci.Id).ToList();
        var outfitId = outfitResult.Outfit.Id == Guid.Empty ? Guid.NewGuid() : outfitResult.Outfit.Id;

        await _outfitRepo.SaveGeneratedOutfitAsync(outfitId, outfitResult.Outfit.Name, outfitResult.Outfit.DateCreated, userId, itemIds, dayStart, cancellationToken);

        return outfitResult;
    }

    public async Task<OutfitGeneratorResult> GenerateOutfitForTodayAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var lat = await _programVars.GetValueAsync("last_location_lat", userId, cancellationToken);
        var lon = await _programVars.GetValueAsync("last_location_lon", userId, cancellationToken);

        if (lat is null || lon is null)
            return Failure("Brak zapisanej lokacji użytkownika. Ustaw lokację aby generować outfity.");

        if (!double.TryParse(lat, CultureInfo.InvariantCulture, out double parsedLat) ||
            !double.TryParse(lon, CultureInfo.InvariantCulture, out double parsedLon))
            return Failure("Zapisana lokacja użytkownika ma niepoprawny format numeryczny.");

        var weather = await _weatherService.GetWeatherForLocationAsync(parsedLat, parsedLon, cancellationToken);

        if (weather is null)
            return Failure("Nie udało się pobrać danych pogodowych.");

        return await GenerateOutfitWithWeatherAsync(userId, weather, cancellationToken);
    }

    public async Task<OutfitGeneratorResult> GenerateOutfitWithWeatherAsync(
        Guid userId,
        WeatherDataForGeneration weather,
        CancellationToken cancellationToken = default)
    {
        var wardrobe = (await _clothingRepo.GetAllAsync(cancellationToken))
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .ToList();

        if (!wardrobe.Any())
            return Failure("Szafa użytkownika jest pusta. Dodaj ubrania aby generować outfity.");

        var threeDaysAgo = DateTime.UtcNow.Date.AddDays(-3);
        var recentUsage = await _usageHistoryRepo.GetByDateRangeAsync(
            userId, threeDaysAgo, DateTime.UtcNow.Date, cancellationToken);

        var recentlyWornIds = recentUsage
            .Where(u => u.Outfit != null)
            .SelectMany(u => u.Outfit.ClothingItems.Select(ci => ci.Id))
            .ToHashSet();

        var requirements = DetermineRequirements(weather);

        for (int fallbackLevel = 0; fallbackLevel <= 5; fallbackLevel++)
        {
            double minimumScore = fallbackLevel switch
            {
                0 => 5.0,
                1 => 3.0,
                2 => 1.0,
                3 => 0.0,
                4 => -5.0,
                _ => double.MinValue
            };

            bool applyColorPenalty = fallbackLevel < 2;

            var result = TryBuildOutfit(
                wardrobe,
                requirements,
                weather,
                recentlyWornIds,
                minimumScore,
                applyColorPenalty);

            if (result != null)
            {
                return new OutfitGeneratorResult
                {
                    Outfit = result,
                    Warnings = BuildWarnings(fallbackLevel, weather),
                    Temperature = weather.Temperature,
                    IsWindy = weather.IsWindy,
                    IsSunny = weather.IsSunny,
                    IsRaining = weather.IsRaining
                };
            }
        }

        return new OutfitGeneratorResult
        {
            Outfit = null,
            Warnings = new List<string> { "Nie można wygenerować outfitu — brak ubrań dla wymaganych slotów." },
            Temperature = weather.Temperature,
            IsWindy = weather.IsWindy,
            IsSunny = weather.IsSunny,
            IsRaining = weather.IsRaining
        };
    }

    private Outfit? TryBuildOutfit(
        List<ClothingItem> wardrobe,
        List<SlotRequirement> requirements,
        WeatherDataForGeneration weather,
        HashSet<Guid> recentlyWornIds,
        double minimumScore,
        bool applyColorPenalty)
    {
        string? targetStyle = PickRandomStyle(wardrobe);

        var outfit = new Outfit
        {
            Id = Guid.NewGuid(),
            Name = $"Generated Outfit {DateTime.Now:HH:mm}",
            DateCreated = DateTime.UtcNow
        };

        var occupiedSlotLayers = new HashSet<SlotLayerKey>();
        int nonNeutralAccumulator = 0;

        foreach (var requirement in requirements)
        {
            int outerLayer = requirement.RequiredLayers.Count > 0 ? requirement.RequiredLayers.Max() : 0;

            foreach (int layer in requirement.RequiredLayers)
            {
                var key = new SlotLayerKey(requirement.SlotName, layer);

                if (occupiedSlotLayers.Contains(key))
                    continue;

                var pool = wardrobe
                    .Where(c => c.Category?.ClothingSlots != null
                             && c.Category.ClothingSlots.Any(s => s.Name == requirement.SlotName)
                             && c.Category.LayerIndex == layer)
                    .ToList();

                if (!pool.Any())
                    return null;

                var scored = pool
                    .Select(item => new ScoredCandidate
                    {
                        Item = item,
                        Score = ScoreItem(
                            item,
                            requirement,
                            weather,
                            recentlyWornIds,
                            targetStyle,
                            isOuterLayer: layer == outerLayer,
                            nonNeutralAccumulator,
                            applyColorPenalty)
                    })
                    .OrderByDescending(s => s.Score)
                    .ToList();

                var aboveThreshold = scored.Where(s => s.Score >= minimumScore).ToList();
                var chosen = aboveThreshold.Any()
                    ? aboveThreshold.Take(3).ToList()[_random.Next(Math.Min(3, aboveThreshold.Count))]
                    : scored.First();

                outfit.ClothingItems.Add(chosen.Item);
                nonNeutralAccumulator += chosen.Item.Colors.Count(c => !c.IsNeutral);

                if (chosen.Item.Category?.ClothingSlots != null)
                {
                    foreach (var coveredSlot in chosen.Item.Category.ClothingSlots)
                    {
                        occupiedSlotLayers.Add(new SlotLayerKey(coveredSlot.Name, layer));
                    }
                }
            }
        }

        return outfit;
    }

    private double ScoreItem(
        ClothingItem item,
        SlotRequirement requirement,
        WeatherDataForGeneration weather,
        HashSet<Guid> recentlyWornIds,
        string? targetStyle,
        bool isOuterLayer,
        int currentNonNeutralCount,
        bool applyColorPenalty)
    {
        double score = 0;

        if (!recentlyWornIds.Contains(item.Id))
            score += ScoreDiversity;

        if (targetStyle != null && item.Styles.Any(s => s.Name == targetStyle))
            score += ScoreStyleMatch;

        if (requirement.RequiredLayers.Count > 0)
        {
            int expectedPerLayer = requirement.MinWarmth / requirement.RequiredLayers.Count;
            int diff = Math.Abs(item.WarmthLevel - expectedPerLayer);
            if (diff <= 3) score += ScoreWarmthIdeal;
            else if (diff <= 6) score += ScoreWarmthAccepted;
        }

        if (isOuterLayer && weather.IsRaining
            && HasProperty(item, "waterproof", "water-resistant", "wodoodpor"))
            score += ScoreWaterproof;

        if (weather.IsWindy && requirement.SlotName == "Głowa"
            && HasProperty(item, "windproof", "wind-resistant", "wiatroszczelny"))
            score += ScoreWindproof;

        if (applyColorPenalty)
        {
            int projected = currentNonNeutralCount + item.Colors.Count(c => !c.IsNeutral);
            if (projected > 1)
                score += (projected - 1) * PenaltyNonNeutralColor;
        }

        return score;
    }

    private static List<SlotRequirement> DetermineRequirements(WeatherDataForGeneration weather)
    {
        var req = new List<SlotRequirement>();

        if (weather.Temperature >= 20)
        {
            req.Add(new SlotRequirement { SlotName = "Core", RequiredLayers = new() { 1 }, MinWarmth = 1 });
            req.Add(new SlotRequirement { SlotName = "Nogi", RequiredLayers = new() { 1 }, MinWarmth = 1 });
        }
        else if (weather.Temperature >= 10)
        {
            req.Add(new SlotRequirement { SlotName = "Core", RequiredLayers = new() { 1, 2 }, MinWarmth = 5 });
            req.Add(new SlotRequirement { SlotName = "Nogi", RequiredLayers = new() { 1 }, MinWarmth = 3 });
        }
        else
        {
            req.Add(new SlotRequirement { SlotName = "Core", RequiredLayers = new() { 1, 2, 3 }, MinWarmth = 12 });
            req.Add(new SlotRequirement { SlotName = "Nogi", RequiredLayers = new() { 1 }, MinWarmth = 5 });
        }

        req.Add(new SlotRequirement { SlotName = "Stopy", RequiredLayers = new() { 1 }, MinWarmth = 1 });

        if (weather.IsWindy || weather.Temperature < 10)
            req.Add(new SlotRequirement { SlotName = "Głowa", RequiredLayers = new() { 1 }, MinWarmth = 0 });

        if (weather.IsSunny)
            req.Add(new SlotRequirement { SlotName = "Oczy", RequiredLayers = new() { 1 }, MinWarmth = 0 });

        return req;
    }

    private static bool HasProperty(ClothingItem item, params string[] keywords) =>
        item.Properties.Any(p =>
            keywords.Any(kw => p.Name.Contains(kw, StringComparison.OrdinalIgnoreCase)));

    private string? PickRandomStyle(List<ClothingItem> wardrobe)
    {
        var styles = wardrobe
            .SelectMany(c => c.Styles)
            .Select(s => s.Name)
            .Distinct()
            .ToList();
        return styles.Any() ? styles[_random.Next(styles.Count)] : null;
    }

    private static List<string> BuildWarnings(int fallbackLevel, WeatherDataForGeneration weather)
    {
        var w = new List<string>();
        if (fallbackLevel >= 1) w.Add("Ostrzeżenie: Może być noszone ubranie z ostatnich 3 dni.");
        if (fallbackLevel >= 2) w.Add("Ostrzeżenie: Outfit może nie mieć harmonijnych kolorów.");
        if (fallbackLevel >= 3) w.Add("Ostrzeżenie: Outfit może być niespójny stylowo.");
        if (fallbackLevel >= 4) w.Add("Ostrzeżenie: Outfit może być niespójny pod kątem ciepła.");
        if (fallbackLevel >= 5 && weather.IsRaining)
            w.Add("Ostrzeżenie: Wybrano odzież nieodporną na deszcz. Zabierz parasol!");
        if (weather.IsWindy && fallbackLevel >= 4)
            w.Add("Ostrzeżenie: Wiatr — czapka może nie być wiatroszczelna. Przytrzymaj!");
        if (weather.IsSunny)
            w.Add("💡 Sugestia: Nie zapomnij okularów przeciwsłonecznych!");
        return w;
    }

    private static OutfitGeneratorResult Failure(string message) =>
        new() { Outfit = null, Warnings = new List<string> { message } };
}