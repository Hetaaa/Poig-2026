using System;
using System.Linq;
using System.Collections.Generic;
using WeatherStyler.Domain.Repositories;

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
                var created = await _attributesRepo.AddClothingSlotAsync(new WeatherStyler.Domain.Wardrobe.Entities.ClothingSlot { Name = s }, cancellationToken);
                slotEntities[s] = created.Id;
            }
            else
            {
                slotEntities[s] = existing.Id;
            }
        }

        // seed colors
        var colors = new[] { "Czarny","Biały","Szary","Granatowy","Beżowy","Brązowy","Szałwiowy","Oliwkowy","Błękitny","Bordowy","Pudrowy Róż","Maślany Żółty","Czerwony" };
        foreach (var c in colors)
        {
            var existing = (await _attributesRepo.GetAllColorsAsync(cancellationToken)).FirstOrDefault(x => x.Name == c);
            if (existing is null)
            {
                await _attributesRepo.AddColorAsync(new WeatherStyler.Domain.Wardrobe.Entities.Color { Name = c }, cancellationToken);
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
            if (existing is null)
            {
                var created = await _attributesRepo.AddCategoryAsync(new WeatherStyler.Domain.Wardrobe.Entities.Category { Name = kv.Key }, cancellationToken);
                categoryId = created.Id;
            }
            else
            {
                categoryId = existing.Id;
            }

            var slotIds = kv.Value.Select(name => slotEntities[name]).ToArray();
            await _attributesRepo.AssociateCategoryWithSlotsAsync(categoryId, slotIds, cancellationToken);
        }
    }
}
