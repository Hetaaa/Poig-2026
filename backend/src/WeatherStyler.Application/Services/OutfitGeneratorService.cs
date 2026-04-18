using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Wardrobe.Entities;

namespace WeatherStyler.Application.Services;

/// <summary>
/// Model wyniku generowania outfitu
/// </summary>
public class OutfitGeneratorResult
{
    public Outfit? Outfit { get; set; }
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Dane pogodowe dla dnia
/// </summary>
public record WeatherDataForGeneration(
    int Temperature,        // Temperatura w °C
    bool IsRaining,        // Czy pada deszcz?
    bool IsWindy,          // Czy jest wietrznie?
    bool IsSunny           // Czy jest słonecznie?
);

/// <summary>
/// Wymagania dla konkretnego slotu
/// </summary>
internal class SlotRequirement
{
    public string SlotName { get; set; } = string.Empty;
    public List<int> RequiredLayers { get; set; } = new();
    public int MinWarmth { get; set; }
    public int MaxWarmth { get; set; } = 100;
}

/// <summary>
/// Serwis generujący outfity dla użytkownika na dzień dzisiejszy
/// Pobiera lokację użytkownika i wywoła WeatherService do pobrania pogody
/// </summary>
public class OutfitGeneratorService
{
    private readonly IProgramVariableRepository _programVars;
    private readonly IClothingItemRepository _clothingRepo;
    private readonly ILookupRepository _lookupRepo;
    private readonly IUsageHistoryRepository _usageHistoryRepo;
    private readonly WeatherService _weatherService;
    private readonly Random _random = new Random();

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

    /// <summary>
    /// Główna metoda generująca outfit na dzień dzisiejszy dla użytkownika
    /// Pobiera lokację użytkownika i pogodę, następnie generuje outfit
    /// </summary>
    public async Task<OutfitGeneratorResult> GenerateOutfitForTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Pobierz lokację użytkownika
        var lat = await _programVars.GetValueAsync("last_location_lat", userId, cancellationToken);
        var lon = await _programVars.GetValueAsync("last_location_lon", userId, cancellationToken);

        if (lat is null || lon is null)
            return new OutfitGeneratorResult 
            { 
                Outfit = null,
                Warnings = new List<string> { "Brak zapisanej lokacji użytkownika. Ustaw lokację aby generować outfity." }
            };

        // Pobierz pogodę dla lokacji
        var weatherJson = await _weatherService.GetWeatherForLocationAsync(double.Parse(lat), double.Parse(lon), cancellationToken);
        if (weatherJson is null)
            return new OutfitGeneratorResult
            {
                Outfit = null,
                Warnings = new List<string> { "Nie udało się pobrać danych pogodowych." }
            };

        // Parsuj pogodę i określ dane dla dzisiaj
        var weather = ParseWeatherForToday(weatherJson);

        // Generuj outfit na podstawie pogody
        return await GenerateOutfitWithWeatherAsync(userId, weather, cancellationToken);
    }

    /// <summary>
    /// Generuj outfit na podstawie wstępnie podanej pogody (dla debug/testing)
    /// </summary>
    public async Task<OutfitGeneratorResult> GenerateOutfitWithWeatherAsync(
        Guid userId, 
        WeatherDataForGeneration weather, 
        CancellationToken cancellationToken = default)
    {
        // Pobierz szafę użytkownika przez repozytorium
        var wardrobe = await _clothingRepo.GetAllAsync(cancellationToken);
        var userClothing = wardrobe
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .ToList();

        if (!userClothing.Any())
            return new OutfitGeneratorResult 
            { 
                Outfit = null,
                Warnings = new List<string> { "Szafa użytkownika jest pusta. Dodaj ubrania aby generować outfity." }
            };

        // Pobierz kategorie z warstwami
        var categories = await _lookupRepo.GetCategoriesAsync(cancellationToken);

        // Pobierz historię noszenia z ostatnich 3 dni dla diversity check
        var threeDaysAgo = DateTime.UtcNow.Date.AddDays(-3);
        var recentUsageHistories = await _usageHistoryRepo.GetByDateRangeAsync(userId, threeDaysAgo, DateTime.UtcNow.Date, cancellationToken);

        // Zbierz ID itemów które były noszone w ostatnich 3 dniach
        var recentlyWornItemIds = recentUsageHistories
            .Where(u => u.Outfit != null)
            .SelectMany(u => u.Outfit.ClothingItems.Select(ci => ci.Id))
            .Distinct()
            .ToHashSet();

        // Określ wymagania na podstawie pogody
        var requirements = DetermineRequirements(weather);

        // Fallback Loop: zaczynamy od maksymalnie restrykcyjnego poziomu 0, kończymy na 5 (brak restrykcji)
        for (int fallbackLevel = 0; fallbackLevel <= 5; fallbackLevel++)
        {
            // Flagi restrykcyjności
            bool enforceDiversity = fallbackLevel < 1;
            bool enforceColorHarmony = fallbackLevel < 2;
            bool enforceStyleConsistency = fallbackLevel < 3;
            bool enforceWarmth = fallbackLevel < 4;
            bool enforceWaterproof = fallbackLevel < 5 && weather.IsRaining;

            // Spróbuj wygenerować outfit na tym poziomie
            var outfit = TryBuildOutfit(
                userClothing, 
                categories,
                requirements,
                weather,
                recentlyWornItemIds,
                enforceDiversity, 
                enforceColorHarmony, 
                enforceStyleConsistency, 
                enforceWarmth, 
                enforceWaterproof);

            if (outfit != null)
            {
                // Sukces! Dodaj ostrzeżenia na podstawie fallback level
                var warnings = GetWarningsForLevel(fallbackLevel, weather);

                return new OutfitGeneratorResult 
                { 
                    Outfit = outfit, 
                    Warnings = warnings 
                };
            }
        }

        // Nie udało się wygenerować outfitu
        return new OutfitGeneratorResult 
        { 
            Outfit = null,
            Warnings = new List<string> { "Nie można wygenerować outfitu spełniającego minimalne wymagania" }
        };
    }

    /// <summary>
    /// Określa wymagania dla slotów na podstawie pogody
    /// </summary>
    private List<SlotRequirement> DetermineRequirements(WeatherDataForGeneration weather)
    {
        var requirements = new List<SlotRequirement>();

        if (weather.Temperature >= 20)
        {
            // Ciepła pogoda: minimalne warstwy
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Core", 
                RequiredLayers = new List<int> { 1 }, 
                MinWarmth = 1,
                MaxWarmth = 100
            });
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Nogi", 
                RequiredLayers = new List<int> { 1 }, 
                MinWarmth = 1,
                MaxWarmth = 100
            });
        }
        else if (weather.Temperature >= 10)
        {
            // Umiarkowana pogoda: 2 warstwy na tułowiu
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Core", 
                RequiredLayers = new List<int> { 1, 2 }, 
                MinWarmth = 5,
                MaxWarmth = 100
            });
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Nogi", 
                RequiredLayers = new List<int> { 1 }, 
                MinWarmth = 3,
                MaxWarmth = 100
            });
        }
        else
        {
            // Zimna pogoda: maksymalne warstwy
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Core", 
                RequiredLayers = new List<int> { 1, 2, 3 }, 
                MinWarmth = 12,
                MaxWarmth = 100
            });
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Nogi", 
                RequiredLayers = new List<int> { 1 }, 
                MinWarmth = 5,
                MaxWarmth = 100
            });
        }

        // Głowa jest opcjonalna - dla wiatru lub zimna
        if (weather.IsWindy || weather.Temperature < 10)
        {
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Głowa", 
                RequiredLayers = new List<int> { 1 }, 
                MinWarmth = 0,
                MaxWarmth = 100
            });
        }

        // Okulary przeciwsłoneczne gdy jest słonecznie
        if (weather.IsSunny)
        {
            requirements.Add(new SlotRequirement 
            { 
                SlotName = "Oczy", 
                RequiredLayers = new List<int> { 1 }, 
                MinWarmth = 0,
                MaxWarmth = 100
            });
        }

        return requirements;
    }

    /// <summary>
    /// Spróbuj wygenerować outfit na danym poziomie restrykcyjności
    /// </summary>
    private Outfit? TryBuildOutfit(
        List<ClothingItem> wardrobe,
        IEnumerable<Category> categories,
        List<SlotRequirement> requirements,
        WeatherDataForGeneration weather,
        HashSet<Guid> recentlyWornItemIds,
        bool enforceDiversity,
        bool enforceColorHarmony,
        bool enforceStyleConsistency,
        bool enforceWarmth,
        bool enforceWaterproof)
    {
        var candidateOutfit = new Outfit 
        { 
            Id = Guid.NewGuid(), 
            Name = $"Generated Outfit {DateTime.Now:HH:mm}", 
            DateCreated = DateTime.UtcNow 
        };

        // Wybierz styl jeśli wymagamy spójności stylistycznej
        string? targetStyle = null;
        if (enforceStyleConsistency)
        {
            var availableStyles = wardrobe
                .SelectMany(c => c.Styles)
                .Select(s => s.Name)
                .Distinct()
                .ToList();

            if (availableStyles.Any())
                targetStyle = availableStyles[_random.Next(availableStyles.Count)];
        }

        // Iteruj przez wymagane sloty i warstwy
        foreach (var requirement in requirements)
        {
            int totalWarmth = 0;

            foreach (int layer in requirement.RequiredLayers)
            {
                // Filtruj dostępne ubrania dla tego slotu i warstwy
                var candidates = wardrobe
                    .Where(c => c.Category.ClothingSlots.Any(s => s.Name == requirement.SlotName) 
                            && c.Category.LayerIndex == layer)
                    .ToList();

                // Filtr: Diversity - nie noszono ostatnio (ostatnie 3 dni)
                if (enforceDiversity)
                {
                    // Pomijaj ubrania noszone w ostatnich 3 dniach
                    candidates = candidates
                        .Where(c => !recentlyWornItemIds.Contains(c.Id))
                        .ToList();
                }

                // Filtr: Style Consistency - dopasuj styl
                if (enforceStyleConsistency && targetStyle != null)
                {
                    candidates = candidates
                        .Where(c => c.Styles.Any(s => s.Name == targetStyle))
                        .ToList();
                }

                // Filtr: Waterproof - dla ostatniej warstwy jeśli pada
                if (enforceWaterproof && layer == requirement.RequiredLayers.Max())
                {
                    candidates = candidates
                        .Where(c => c.Properties.Any(p => p.Name.ToLower().Contains("waterproof") 
                                                      || p.Name.ToLower().Contains("water-resistant")
                                                      || p.Name.ToLower().Contains("wodoodpor")))
                        .ToList();
                }

                // Filtr: Windproof - dla głowy jeśli jest wietrznie
                if (weather.IsWindy && requirement.SlotName == "Głowa")
                {
                    candidates = candidates
                        .Where(c => c.Properties.Any(p => p.Name.ToLower().Contains("windproof") 
                                                      || p.Name.ToLower().Contains("wind-resistant")
                                                      || p.Name.ToLower().Contains("wiatroszczelny")))
                        .ToList();

                    // Jeśli brak windproof, akceptuj bez tego filtra na wyższych fallback levelach
                    if (!candidates.Any() && weather.IsWindy)
                    {
                        candidates = wardrobe
                            .Where(c => c.Category.ClothingSlots.Any(s => s.Name == requirement.SlotName) 
                                    && c.Category.LayerIndex == layer)
                            .ToList();
                    }
                }

                // KRYTYCZNE: Jeśli brak kandydatów, przerwij budowanie outfitu
                if (!candidates.Any())
                    return null;

                // Wybierz losowe ubranie z przefiltrowanej listy
                var selected = candidates.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (selected == null)
                    return null;

                candidateOutfit.ClothingItems.Add(selected);
                totalWarmth += selected.WarmthLevel;
            }

            // Walidacja ciepła dla slotu
            if (enforceWarmth && (totalWarmth < requirement.MinWarmth || totalWarmth > requirement.MaxWarmth))
                return null;
        }

        // Walidacja harmonii kolorów
        if (enforceColorHarmony && !ValidateColorHarmony(candidateOutfit.ClothingItems))
            return null;

        return candidateOutfit;
    }

    /// <summary>
    /// Waliduj harmonię kolorów - maksymalnie jeden kolor nie-neutralny
    /// </summary>
    private bool ValidateColorHarmony(ICollection<ClothingItem> items)
    {
        var nonNeutralCount = items
            .SelectMany(i => i.Colors)
            .Where(c => !c.IsNeutral)
            .Distinct()
            .Count();

        // Maksymalnie jeden akcent kolorystyczny (kolor nie-neutralny)
        return nonNeutralCount <= 1;
    }

    /// <summary>
    /// Zwróć ostrzeżenia na podstawie poziomu fallback
    /// </summary>
    private List<string> GetWarningsForLevel(int fallbackLevel, WeatherDataForGeneration weather)
    {
        var warnings = new List<string>();

        if (fallbackLevel >= 1)
            warnings.Add("Ostrzeżenie: Może być noszone ubranie z ostatnich 3 dni.");

        if (fallbackLevel >= 2)
            warnings.Add("Ostrzeżenie: Outfit może nie mieć harmonijnych kolorów.");

        if (fallbackLevel >= 3)
            warnings.Add("Ostrzeżenie: Outfit może być niespójny stylowo.");

        if (fallbackLevel >= 4)
            warnings.Add("Ostrzeżenie: Outfit może nie spełniać wymagań ciepła dla pogody.");

        if (fallbackLevel >= 5 && weather.IsRaining)
            warnings.Add("Ostrzeżenie: Wybrano odzież nieodporną na deszcz. Zabierz parasol!");

        if (weather.IsWindy && fallbackLevel >= 4)
            warnings.Add("Ostrzeżenie: Wiatr - Czapka może nie być wiatroszczelna. Przytrzymaj!");

        if (weather.IsSunny)
            warnings.Add("💡 Sugestia: Nie zapomnij okularów przeciwsłonecznych!");

        return warnings;
    }

    /// <summary>
    /// Parsuj JSON pogody i zwróć WeatherDataForGeneration dla dzisiaj
    /// </summary>
    private WeatherDataForGeneration ParseWeatherForToday(string weatherJson)
    {
        // Simplified parsing - w produkcji użyć System.Text.Json
        var hasRain = weatherJson.Contains("\"precipitation_sum\":[");
        var hasCloud = weatherJson.Contains("\"cloudcover\":");
        var isSunny = !hasCloud || weatherJson.Contains("\"cloudcover\":[0") || weatherJson.Contains("\"cloudcover\":[1") || weatherJson.Contains("\"cloudcover\":[5");
        var temperature = 15; // default

        return new WeatherDataForGeneration(temperature, hasRain, false, isSunny);
    }
}
