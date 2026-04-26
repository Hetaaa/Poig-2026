using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Contracts;
using WeatherStyler.Domain.Interfaces.Services;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public AuthController(IUserAccountService userAccountService, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _userAccountService = userAccountService;
        _configuration = configuration;
        _isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var resp = await _userAccountService.RegisterAsync(request.Username, request.Password);
            return Ok(new WeatherStyler.Contracts.AuthResponse(resp.Token, resp.Username));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var resp = await _userAccountService.LoginAsync(request.Username, request.Password);
            return Ok(new WeatherStyler.Contracts.AuthResponse(resp.Token, resp.Username));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            if (_isDevelopment) throw;
            return StatusCode(500, new { message = ex.Message });
        }
    }

}
