namespace Celeste.Inventory.Api.Authorization;

/// <summary>
///     Defines authorization policy names used by the API.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    ///     Gets the manufacturer read policy name.
    /// </summary>
    public const string ManufacturerRead = nameof(ManufacturerRead);

    /// <summary>
    ///     Gets the manufacturer write policy name.
    /// </summary>
    public const string ManufacturerWrite = nameof(ManufacturerWrite);
}
