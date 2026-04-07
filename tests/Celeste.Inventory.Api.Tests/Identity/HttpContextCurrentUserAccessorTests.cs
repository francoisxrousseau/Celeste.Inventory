namespace Celeste.Inventory.Api.Tests.Identity;

using System.Security.Claims;
using Celeste.Inventory.Api.Identity;
using ApiAuthenticationOptions = Celeste.Inventory.Api.Options.AuthenticationOptions;
using Microsoft.AspNetCore.Http;

/// <summary>
///     Covers HTTP-backed current-user access behavior.
/// </summary>
public sealed class HttpContextCurrentUserAccessorTests
{
    /// <summary>
    ///     Ensures configured claim types are used for user id and display name resolution.
    /// </summary>
    [Fact]
    public void CurrentUserAccessor_UsesConfiguredClaimTypes()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim("custom-user-id", "user-123"),
                new Claim("custom-name", "celeste-admin")
            ],
            "Test"));

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = Microsoft.Extensions.Options.Options.Create(new ApiAuthenticationOptions
        {
            Authority = "http://localhost:8080/realms/celeste",
            Audience = "celeste.inventory",
            RequireHttpsMetadata = false,
            UserIdClaimType = "custom-user-id",
            NameClaimType = "custom-name"
        });

        var currentUserAccessor = new HttpContextCurrentUserAccessor(httpContextAccessor, options);

        Assert.Equal("user-123", currentUserAccessor.UserId);
        Assert.Equal("celeste-admin", currentUserAccessor.Name);
    }
}
