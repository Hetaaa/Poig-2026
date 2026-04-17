using System;

namespace WeatherStyler.Application.Services;

public interface IUserService
{
    Guid GetUserId();
    Guid? GetUserIdOrNull();
}
