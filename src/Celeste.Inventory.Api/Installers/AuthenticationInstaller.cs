namespace Celeste.Inventory.Api.Installers;

using Celeste.Inventory.Api.Authorization;
using Celeste.Inventory.Api.Identity;
using Celeste.Inventory.Api.Options;
using Celeste.Inventory.Core.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

/// <summary>
///     Registers authentication and authorization services for the API.
/// </summary>
public static class AuthenticationInstaller
{
    /// <summary>
    ///     Adds authentication and authorization services using configuration-driven JWT bearer options.
    /// </summary>
    /// <param name="services">
    ///     The service collection being configured.
    /// </param>
    /// <param name="configuration">
    ///     The application configuration root.
    /// </param>
    /// <returns>
    ///     The same service collection for chaining.
    /// </returns>
    public static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AuthenticationOptions>()
            .Bind(configuration.GetSection(AuthenticationOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Authority), "Authentication authority is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Authentication audience is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.NameClaimType), "Authentication name claim type is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.UserIdClaimType), "Authentication user id claim type is required.")
            .ValidateOnStart();

        var authenticationOptions = configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>()
            ?? new AuthenticationOptions();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.Authority = authenticationOptions.Authority;
                options.Audience = authenticationOptions.Audience;
                options.RequireHttpsMetadata = authenticationOptions.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = authenticationOptions.NameClaimType
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.ManufacturerRead,
                policy => policy.RequireAuthenticatedUser().RequireScope("manufacturer.read"));

            options.AddPolicy(
                AuthorizationPolicies.ManufacturerWrite,
                policy => policy.RequireAuthenticatedUser().RequireScope("manufacturer.write"));

            options.AddPolicy(
                AuthorizationPolicies.ProductRead,
                policy => policy.RequireAuthenticatedUser().RequireScope("product.read"));

            options.AddPolicy(
                AuthorizationPolicies.ProductWrite,
                policy => policy.RequireAuthenticatedUser().RequireScope("product.write"));
        });

        return services;
    }
}
