using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WeatherStyler.Infrastructure.Entities;

public class UserExistsRequirement : IAuthorizationRequirement { }

public class UserExistsHandler : AuthorizationHandler<UserExistsRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserExistsHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserExistsRequirement requirement)
    {
        var sub = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var userId))
        {
            context.Fail();
            return;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user != null)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}