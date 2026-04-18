using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WeatherStyler.Infrastructure.Entities;
using WeatherStyler.Application.Services;

namespace WeatherStyler.Infrastructure.Services;

internal class UserAccountService : IUserAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public UserAccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(string username, string password)
    {
        var existing = await _userManager.FindByNameAsync(username);
        if (existing is not null)
            throw new InvalidOperationException("Username already exists");

        var user = new ApplicationUser
        {
            UserName = username,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(';', result.Errors.Select(e => e.Description)));

        var token = await GenerateJwtTokenAsync(user);
        return new AuthResult(token, user.UserName ?? string.Empty);
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
            throw new InvalidOperationException("Invalid credentials");

        var valid = await _userManager.CheckPasswordAsync(user, password);
        if (!valid)
            throw new InvalidOperationException("Invalid credentials");

        var token = await GenerateJwtTokenAsync(user);
        return new AuthResult(token, user.UserName ?? string.Empty);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WeatherStyler";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var expires = DateTime.UtcNow.AddDays(1);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: null,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
