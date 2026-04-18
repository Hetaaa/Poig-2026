using System;
using System.Threading.Tasks;

namespace WeatherStyler.Application.Services;

public record AuthResult(string Token, string Username);

public interface IUserAccountService
{
    Task<AuthResult> RegisterAsync(string username, string password);
    Task<AuthResult> LoginAsync(string username, string password);
}
