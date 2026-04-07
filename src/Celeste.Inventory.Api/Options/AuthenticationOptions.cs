namespace Celeste.Inventory.Api.Options;

/// <summary>
///     Represents configuration used to validate bearer tokens and map identity claims.
/// </summary>
public sealed class AuthenticationOptions
{
    /// <summary>
    ///     Gets the configuration section name for authentication options.
    /// </summary>
    public const string SectionName = "Authentication";

    /// <summary>
    ///     Gets or sets the token issuer authority URL.
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the expected token audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether metadata must be loaded over HTTPS.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    ///     Gets or sets the claim type used for the authenticated principal name.
    /// </summary>
    public string NameClaimType { get; set; } = "preferred_username";

    /// <summary>
    ///     Gets or sets the claim type used for the authenticated user identifier.
    /// </summary>
    public string UserIdClaimType { get; set; } = "sub";
}
