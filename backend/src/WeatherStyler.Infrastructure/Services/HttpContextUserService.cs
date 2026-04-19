using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WeatherStyler.Application.Services;

namespace WeatherStyler.Infrastructure.Services;

internal class HttpContextUserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || user.Identity is null || !user.Identity.IsAuthenticated)
            ;

        // Try common claim types in order: JWT 'sub', NameIdentifier, 'id'
        var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("id")?.Value
            ?? user.FindFirst("user_id")?.Value;

        if (Guid.TryParse(sub, out var id)) return id;

        // Fallback: try to read Authorization header and parse JWT without validating signature
        try
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    var sub2 = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                        ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        ?? jwt.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                    if (Guid.TryParse(sub2, out var id2)) return id2;
                }
            }
        }
        catch
        {
            // ignore parsing errors
        }

        throw new InvalidOperationException("Unable to determine current user id");
    }

    public Guid? GetUserIdOrNull()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || user.Identity is null || !user.Identity.IsAuthenticated)
        {
            // try Authorization header fallback
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwt = handler.ReadJwtToken(token);
                        var sub2 = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                            ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                            ?? jwt.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                        if (Guid.TryParse(sub2, out var id2)) return id2;
                    }
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }

        var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("id")?.Value
            ?? user.FindFirst("user_id")?.Value;

        if (Guid.TryParse(sub, out var id)) return id;
        return null;
    }
}
