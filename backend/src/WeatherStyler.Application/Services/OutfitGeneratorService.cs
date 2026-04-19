using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Entities.BuisnessLogic;

namespace WeatherStyler.Application.Services;

/// <summary>
/// Wymagania dla konkretnego slotu ciała (np. "Core" z warstwami [1,2,3])
/// </summary>
internal class SlotRequirement
{
    public string SlotName { get; set; } = string.Empty;
    public List<int> RequiredLayers { get; set; } = new();
    public int MinWarmth { get; set; }
    public int MaxWarmth { get; set; } = 100;
}

/// <summary>
/// Klucz unikalnie identyfikujący zajęcie fizycznego slotu na konkretnej warstwie.
/// Np. ("Core", 1), ("Nogi", 1), ("Core", 2)
/// Używany do wykrywania kolizji: sukienka zajmująca Core+Nogi/warstwa-1
/// blokuje oba te klucze — spodnie na warstwę 1 nie zostaną już dobrane.
/// </summary>
internal readonly record struct SlotLayerKey(string SlotName, int Layer);

/// <summary>
/// Wewnętrzny wynik punktacji kandydata na ubranie
/// </summary>
internal class ScoredCandidate
{
    public ClothingItem Item { get; set; } = null!;
    public double Score { get; set; }
}

/// <summary>
/// Serwis generujący outfity dla użytkownika na dzień dzisiejszy.
///
/// KLUCZOWA LOGIKA SLOTÓW — zapobieganie kolizjom:
/// ─────────────────────────────────────────────────
/// Każde ubranie należy do kategorii, która może zajmować WIELE fizycznych slotów
/// (np. sukienka → ClothingSlots = [Core, Nogi], LayerIndex = 1).
///
/// Gdy ubranie zostanie wybrane, wszystkie jego sloty na tej warstwie trafiają
/// do zbioru `occupiedSlotLayers`. Kolejne wymagania których klucz (slot, warstwa)
/// jest już w tym zbiorze są pomijane — nie szukamy drugiego ubrania na to miejsce.
///
/// Przykład przy temp < 10°C (wymagania: Core/1,2,3 + Nogi/1):
///   Iteracja Core/warstwa-1 → kandydaci: koszulki + sukienki
///     → wybrano sukienkę (ClothingSlots=[Core,Nogi], LayerIndex=1)
///     → occupied: {(Core,1), (Nogi,1)}
///   Iteracja Core/warstwa-2 → (Core,2) wolne → szukamy swetra ✓
///   Iteracja Core/warstwa-3 → (Core,3) wolne → szukamy kurtki ✓
///   Iteracja Nogi/warstwa-1 → (Nogi,1) ZAJĘTE → skip (brak spodni!) ✓
///
/// STRATEGIA JAKOŚCI (scoring zamiast hard-filtering):
/// ─────────────────────────────────────────────────────
/// Zamiast twardych filtrów, każde ubranie dostaje wynik punktowy.
/// Fallback stopniowo obniża minimalny akceptowany próg — dzięki temu
/// przy małej szafie outfit prawie zawsze zostaje zbudowany.
/// Twarde return null tylko gdy pula dla slotu/warstwy jest dosłownie pusta.
/// </summary>
public class OutfitGeneratorService
{
    private readonly IProgramVariableRepository _programVars;
    private readonly IClothingItemRepository _clothingRepo;
    private readonly ILookupRepository _lookupRepo;
    private readonly IUsageHistoryRepository _usageHistoryRepo;
    private readonly WeatherService _weatherService;
    private readonly Random _random = new Random();

    // Wagi punktów dla kryteriów jakościowych
    private const double ScoreDiversity = 3.0;  // nie noszono w ostatnich 3 dniach
    private const double ScoreStyleMatch = 2.0;  // pasuje do wylosowanego stylu
    private const double ScoreWarmthIdeal = 2.0;  // ciepłota w oknie ±3
    private const double ScoreWarmthAccepted = 0.5;  // ciepłota w oknie ±6
    private const double ScoreWaterproof = 4.0;  // wodoodporne gdy pada
    private const double ScoreWindproof = 3.0;  // wiatroszczelne gdy wieje
    private const double PenaltyNonNeutralColor = -2.0;  // każdy nadmiarowy kolor nie-neutralny

    public OutfitGeneratorService(
        IProgramVariableRepository programVars,
        IClothingItemRepository clothingRepo,
        ILookupRepository lookupRepo,
        IUsageHistoryRepository usageHistoryRepo,
        WeatherService weatherService)
    {
        _programVars = programVars;
        _clothingRepo = clothingRepo;
        _lookupRepo = lookupRepo;
        _usageHistoryRepo = usageHistoryRepo;
        _weatherService = weatherService;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Publiczne API
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<OutfitGeneratorResult> GenerateOutfitForTodayAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var lat = await _programVars.GetValueAsync("last_location_lat", userId, cancellationToken);
        var lon = await _programVars.GetValueAsync("last_location_lon", userId, cancellationToken);

        if (lat is null || lon is null)
            return Failure("Brak zapisanej lokacji użytkownika. Ustaw lokację aby generować outfity.");

        var weather = await _weatherService.GetWeatherForLocationAsync(
            double.Parse(lat), double.Parse(lon), cancellationToken);

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
            // Minimalny wynik punktowy kandydata — spada z każdym poziomem fallbacku
            double minimumScore = fallbackLevel switch
            {
                0 => 5.0,
                1 => 3.0,
                2 => 1.0,
                3 => 0.0,
                4 => -5.0,
                _ => double.MinValue
            };

            // Kara kolorystyczna aktywna tylko na poziomach 0–1
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

    // ──────────────────────────────────────────────────────────────────────────
    // Budowanie outfitu z respektowaniem kolizji slotów
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Próbuje zbudować outfit. Zwraca null tylko gdy pula dla jakiegoś
    /// wymaganego (i niezajętego) slotu/warstwy jest dosłownie pusta.
    /// </summary>
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

        // Zbiór par (slot, warstwa) już zajętych przez wybrane ubrania.
        // Gdy sukienka zajmuje Core+Nogi na warstwie 1, obie pary trafiają tutaj.
        var occupiedSlotLayers = new HashSet<SlotLayerKey>();

        // Licznik non-neutral dla harmonii kolorów
        int nonNeutralAccumulator = 0;

        foreach (var requirement in requirements)
        {
            int outerLayer = requirement.RequiredLayers.Max();

            foreach (int layer in requirement.RequiredLayers)
            {
                var key = new SlotLayerKey(requirement.SlotName, layer);

                // Ten slot+warstwa już zajęty (np. sukienka "wzięła" Nogi/1) — skip
                if (occupiedSlotLayers.Contains(key))
                    continue;

                // Pula kandydatów: ubrania pasujące do tego konkretnego slotu i warstwy
                var pool = wardrobe
                    .Where(c => c.Category?.ClothingSlots != null
                             && c.Category.ClothingSlots.Any(s => s.Name == requirement.SlotName)
                             && c.Category.LayerIndex == layer)
                    .ToList();

                // Brak ubrań w szafie dla tego slotu/warstwy — nie da się naprawić fallbackiem
                if (!pool.Any())
                    return null;

                // Oceń i posortuj kandydatów
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

                // Wybierz losowo z Top-3 nad progiem; jeśli nikt progu nie spełnia — weź najlepszego
                var aboveThreshold = scored.Where(s => s.Score >= minimumScore).ToList();
                var chosen = aboveThreshold.Any()
                    ? aboveThreshold.Take(3).ToList()[_random.Next(Math.Min(3, aboveThreshold.Count))]
                    : scored.First();

                outfit.ClothingItems.Add(chosen.Item);
                nonNeutralAccumulator += chosen.Item.Colors.Count(c => !c.IsNeutral);

                // Zarejestruj WSZYSTKIE sloty zajmowane przez wybrane ubranie na tej warstwie.
                //
                // Dlaczego wszystkie sloty kategorii, nie tylko bieżący?
                // Bo sukienka (ClothingSlots=[Core,Nogi]) wybrana przy okazji szukania
                // ubrania na slot Core/warstwa-1, pokrywa RÓWNIEŻ Nogi/warstwa-1.
                // Bez tego moglibyśmy później dobrać spodnie na te same nogi.
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

    // ──────────────────────────────────────────────────────────────────────────
    // Scoring
    // ──────────────────────────────────────────────────────────────────────────

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

    // ──────────────────────────────────────────────────────────────────────────
    // Wymagania pogodowe
    // ──────────────────────────────────────────────────────────────────────────

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

        if (weather.IsWindy || weather.Temperature < 10)
            req.Add(new SlotRequirement { SlotName = "Głowa", RequiredLayers = new() { 1 }, MinWarmth = 0 });

        if (weather.IsSunny)
            req.Add(new SlotRequirement { SlotName = "Oczy", RequiredLayers = new() { 1 }, MinWarmth = 0 });

        return req;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpery
    // ──────────────────────────────────────────────────────────────────────────

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
        if (fallbackLevel >= 4) w.Add("Ostrzeżenie: Outfit może nie spełniać optymalnych wymagań ciepła.");
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