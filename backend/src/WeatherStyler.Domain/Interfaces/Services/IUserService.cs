using System;

namespace WeatherStyler.Domain.Interfaces.Services;

public interface IUserService
{
    Guid GetUserId();
    Guid? GetUserIdOrNull();
}
