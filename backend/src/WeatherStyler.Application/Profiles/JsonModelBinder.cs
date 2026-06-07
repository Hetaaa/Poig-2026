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
            var value = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    // Używamy ctx.ModelType, dzięki czemu ten sam binder zadziała do Guidów, Properties i wszystkiego innego!
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parsed = JsonSerializer.Deserialize(value, ctx.ModelType, options);
                    ctx.Result = ModelBindingResult.Success(parsed);
                }
                catch
                {
                    ctx.ModelState.TryAddModelError(ctx.ModelName, "Nieprawidłowy format JSON.");
                }
            }
            return Task.CompletedTask;
        }
    }
}
