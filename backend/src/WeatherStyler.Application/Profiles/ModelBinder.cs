using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WeatherStyler.Application.Profiles
{
    public class GuidArrayBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext ctx)
        {
            var value = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;
            if (!string.IsNullOrEmpty(value))
            {
                var parsed = JsonSerializer.Deserialize<IEnumerable<Guid>>(value);
                ctx.Result = ModelBindingResult.Success(parsed);
            }
            return Task.CompletedTask;
        }
    }
}
