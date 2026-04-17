using System;
using Microsoft.AspNetCore.Http;
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
        var sub = _httpContextAccessor.HttpContext?.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (Guid.TryParse(sub, out var id)) return id;
        throw new InvalidOperationException("Unable to determine current user id");
    }

    public Guid? GetUserIdOrNull()
    {
        var sub = _httpContextAccessor.HttpContext?.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (Guid.TryParse(sub, out var id)) return id;
        return null;
    }
}
