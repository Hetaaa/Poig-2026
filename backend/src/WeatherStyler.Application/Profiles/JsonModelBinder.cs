using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WeatherStyler.Application.Profiles
{
    public class JsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext ctx)
        {
            var valueResult = ctx.ValueProvider.GetValue(ctx.ModelName);
            if (valueResult == ValueProviderResult.None) return Task.CompletedTask;

            var value = valueResult.FirstValue;
            if (string.IsNullOrWhiteSpace(value)) return Task.CompletedTask;

            try
            {
                // SCENARIUSZ 1: Jeśli tekst zaczyna się od '[' lub '{', to jest to prawdziwy JSON (np. ze Scalara lub dla Properties)
                if (value.TrimStart().StartsWith("[") || value.TrimStart().StartsWith("{"))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parsed = JsonSerializer.Deserialize(value, ctx.ModelType, options);
                    ctx.Result = ModelBindingResult.Success(parsed);
                }
                // SCENARIUSZ 2: To surowy tekst przesyłany z tradycyjnego FormData (np. z Axios)
                else
                {
                    // Jeśli celem jest lista Guidów (StyleIds, ColorIds)
                    if (ctx.ModelType == typeof(IEnumerable<Guid>))
                    {
                        // Parsujemy wszystkie przesłane pod tym kluczem wartości ignorując błędy
                        var guids = valueResult
                            .Select(v => Guid.TryParse(v, out var g) ? g : Guid.Empty)
                            .Where(g => g != Guid.Empty)
                            .ToList();

                        ctx.Result = ModelBindingResult.Success(guids);
                    }
                }
            }
            catch (Exception ex)
            {
                // Zwracamy czytelny błąd zamiast wywalać aplikację
                ctx.ModelState.TryAddModelError(ctx.ModelName, $"Błąd parsowania danych: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
