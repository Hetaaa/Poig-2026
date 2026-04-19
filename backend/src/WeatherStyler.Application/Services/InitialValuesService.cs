using System;
using System.Linq;
using System.Collections.Generic;
using WeatherStyler.Domain.Repositories;
using WeatherStyler.Domain.Entities;

namespace WeatherStyler.Application.Services;

public class InitialValuesService
{
    private readonly IClothingAttributesRepository _attributesRepo;
    private readonly ILookupRepository _lookupRepo;

    public InitialValuesService(IClothingAttributesRepository attributesRepo, ILookupRepository lookupRepo)
    {
        _attributesRepo = attributesRepo;
        _lookupRepo = lookupRepo;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // seed clothing slots
        var slots = new[] { "Głowa", "Core", "Ręce", "Nogi" };
        var slotEntities = new Dictionary<string, Guid>();
        foreach (var s in slots)
        {
            var existing = await _attributesRepo.GetClothingSlotByNameAsync(s, cancellationToken);
            if (existing is null)
            {
                var created = await _attributesRepo.AddClothingSlotAsync(new WeatherStyler.Domain.Entities.ClothingSlot { Name = s }, cancellationToken);
                slotEntities[s] = created.Id;
            }
            else
            {
                slotEntities[s] = existing.Id;
            }
        }

        // seed colors (mark neutral colors)
        var colors = new[] { "Czarny","Biały","Szary","Granatowy","Beżowy","Brązowy","Szałwiowy","Oliwkowy","Błękitny","Bordowy","Pudrowy Róż","Maślany Żółty","Czerwony" };
        var neutralColorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Czarny", "Biały", "Szary", "Beżowy", "Brązowy" };
        foreach (var c in colors)
        {
            var existing = (await _attributesRepo.GetAllColorsAsync(cancellationToken)).FirstOrDefault(x => x.Name == c);
            var isNeutral = neutralColorNames.Contains(c);
            if (existing is null)
            {
                await _attributesRepo.AddColorAsync(new WeatherStyler.Domain.Entities.Color { Name = c, IsNeutral = isNeutral }, cancellationToken);
            }
            else if (existing.IsNeutral != isNeutral)
            {
                // normalize IsNeutral flag
                existing.IsNeutral = isNeutral;
                await _attributesRepo.UpdateColorAsync(new WeatherStyler.Domain.Entities.Color { Id = existing.Id, Name = existing.Name, IsNeutral = existing.IsNeutral }, cancellationToken);
            }
        }

        // seed styles
        var styles = new[] { "Casual", "Sportowy", "Elegancki" };
        foreach (var s in styles)
        {
            var existingStyle = (await _attributesRepo.GetAllStylesAsync(cancellationToken)).FirstOrDefault(x => x.Name == s);
            if (existingStyle is null)
            {
                await _attributesRepo.AddStyleAsync(new WeatherStyler.Domain.Entities.Style { Name = s }, cancellationToken);
            }
        }

        // seed categories with slot associations
        var categoryMap = new Dictionary<string, string[]>
        {
            ["T-shirt"] = new[] { "Core", "Ręce" },
            ["Bluza"] = new[] { "Core", "Ręce" },
            ["Kurtka"] = new[] { "Core", "Ręce" },
            ["Płaszcz"] = new[] { "Core", "Ręce" },
            ["Koszula"] = new[] { "Core", "Ręce" },
            ["Sweter"] = new[] { "Core", "Ręce" },
            ["Marynarka"] = new[] { "Core", "Ręce" },
            ["Jeansy"] = new[] { "Nogi" },
            ["Szorty"] = new[] { "Nogi" },
            ["Spódnica"] = new[] { "Nogi" },
            ["Sukienka"] = new[] { "Core", "Nogi" },
            ["Kombinezon"] = new[] { "Core", "Ręce", "Nogi" },
            ["Kamizelka"] = new[] { "Core" },
            ["Top na ramiączkach"] = new[] { "Core" },
            ["Czapka"] = new[] { "Głowa" },
            ["Kapelusz"] = new[] { "Głowa" },
            ["Legginsy"] = new[] { "Nogi" },
            ["Kardigan"] = new[] { "Core", "Ręce" },
            ["Spodnie dresowe"] = new[] { "Nogi" },
            ["Body"] = new[] { "Core" }
        };

        foreach (var kv in categoryMap)
        {
            var existing = (await _attributesRepo.GetAllCategoriesAsync(cancellationToken)).FirstOrDefault(x => x.Name == kv.Key);
            Guid categoryId;

            // determine sensible layer index
            int layerIndex = 1;
            if (kv.Key.IndexOf("Kurtka", StringComparison.OrdinalIgnoreCase) >= 0 || kv.Key.IndexOf("Płaszcz", StringComparison.OrdinalIgnoreCase) >= 0 || kv.Key.IndexOf("Marynarka", StringComparison.OrdinalIgnoreCase) >= 0)
                layerIndex = 3;
            else if (kv.Key.IndexOf("Sweter", StringComparison.OrdinalIgnoreCase) >= 0 || kv.Key.IndexOf("Bluza", StringComparison.OrdinalIgnoreCase) >= 0 || kv.Key.IndexOf("Kardigan", StringComparison.OrdinalIgnoreCase) >= 0 || kv.Key.IndexOf("Kamizelka", StringComparison.OrdinalIgnoreCase) >= 0)
                layerIndex = 2;

            if (existing is null)
            {
                var created = await _attributesRepo.AddCategoryAsync(new WeatherStyler.Domain.Entities.Category { Name = kv.Key, LayerIndex = layerIndex }, cancellationToken);
                categoryId = created.Id;
            }
            else
            {
                categoryId = existing.Id;
                if (existing.LayerIndex != layerIndex)
                {
                    existing.LayerIndex = layerIndex;
                    await _attributesRepo.UpdateCategoryAsync(new WeatherStyler.Domain.Entities.Category { Id = existing.Id, Name = existing.Name, LayerIndex = existing.LayerIndex }, cancellationToken);
                }
            }

            var slotIds = kv.Value.Select(name => slotEntities[name]).ToArray();
            await _attributesRepo.AssociateCategoryWithSlotsAsync(categoryId, slotIds, cancellationToken);
        }

        // optionally create a default style associations or other lookups if necessary
    }
}
