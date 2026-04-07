namespace Celeste.Inventory.Core.Identity;

/// <summary>
///     Exposes the authenticated user context to application services.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    ///     Gets the stable authenticated user identifier.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    ///     Gets the authenticated user display name when available.
    /// </summary>
    string? Name { get; }
}
