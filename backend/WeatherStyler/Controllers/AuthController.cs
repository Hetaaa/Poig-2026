using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStyler.Contracts;

namespace WeatherStyler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly WeatherStyler.Application.Services.IUserAccountService _userAccountService;

    public AuthController(WeatherStyler.Application.Services.IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
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
    }

}
