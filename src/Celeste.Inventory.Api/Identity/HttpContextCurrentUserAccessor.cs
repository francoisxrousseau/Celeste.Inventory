namespace Celeste.Inventory.Api.Identity;

using Celeste.Inventory.Api.Options;
using Celeste.Inventory.Core.Identity;
using Microsoft.Extensions.Options;

/// <summary>
///     Resolves the authenticated user from the current HTTP context.
/// </summary>
public sealed class HttpContextCurrentUserAccessor(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthenticationOptions> authenticationOptions) : ICurrentUserAccessor
{
    /// <summary>
    ///     Gets the stable authenticated user identifier.
    /// </summary>
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirst(authenticationOptions.Value.UserIdClaimType)?.Value;

    /// <summary>
    ///     Gets the authenticated user display name when available.
    /// </summary>
    public string? Name => httpContextAccessor.HttpContext?.User.FindFirst(authenticationOptions.Value.NameClaimType)?.Value;
}
