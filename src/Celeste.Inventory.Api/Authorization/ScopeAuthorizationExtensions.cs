namespace Celeste.Inventory.Api.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

/// <summary>
///     Provides helpers for scope-based authorization policies.
/// </summary>
public static class ScopeAuthorizationExtensions
{
    /// <summary>
    ///     Adds a scope requirement that supports Keycloak space-separated scope claims.
    /// </summary>
    /// <param name="policy">
    ///     The authorization policy being configured.
    /// </param>
    /// <param name="requiredScope">
    ///     The scope required for the policy.
    /// </param>
    /// <returns>
    ///     The same policy builder for chaining.
    /// </returns>
    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder policy, string requiredScope)
    {
        return policy.RequireAssertion(context => HasScope(context.User, requiredScope));
    }

    /// <summary>
    ///     Determines whether the principal has the specified scope.
    /// </summary>
    /// <param name="user">
    ///     The authenticated principal.
    /// </param>
    /// <param name="requiredScope">
    ///     The scope required for access.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when the required scope is present; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool HasScope(ClaimsPrincipal user, string requiredScope)
    {
        return user.FindAll("scope")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Contains(requiredScope, StringComparer.Ordinal);
    }
}
