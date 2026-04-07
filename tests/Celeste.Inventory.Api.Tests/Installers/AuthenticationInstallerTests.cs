namespace Celeste.Inventory.Api.Tests.Installers;

using Celeste.Inventory.Api.Installers;
using Celeste.Inventory.Api.Authorization;
using ApiAuthenticationOptions = Options.AuthenticationOptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
///     Covers authentication service registration behavior.
/// </summary>
public sealed class AuthenticationInstallerTests
{
    /// <summary>
    ///     Ensures authentication options bind and bearer becomes the default scheme.
    /// </summary>
    [Fact]
    public async Task AddAuthenticationAuthorization_BindsOptionsAndRegistersJwtBearerDefaults()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiAuthenticationOptions.SectionName}:Authority"] = "http://localhost:8080/realms/celeste",
                [$"{ApiAuthenticationOptions.SectionName}:Audience"] = "celeste.inventory",
                [$"{ApiAuthenticationOptions.SectionName}:RequireHttpsMetadata"] = "false",
                [$"{ApiAuthenticationOptions.SectionName}:NameClaimType"] = "preferred_username",
                [$"{ApiAuthenticationOptions.SectionName}:UserIdClaimType"] = "sub"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddAuthenticationAuthorization(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;
        var schemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();

        Assert.Equal("http://localhost:8080/realms/celeste", options.Authority);
        Assert.Equal("celeste.inventory", options.Audience);
        Assert.False(options.RequireHttpsMetadata);
        Assert.Equal("preferred_username", options.NameClaimType);
        Assert.Equal("sub", options.UserIdClaimType);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, (await schemeProvider.GetDefaultAuthenticateSchemeAsync())?.Name);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, (await schemeProvider.GetDefaultChallengeSchemeAsync())?.Name);
    }

    /// <summary>
    ///     Ensures JWT bearer options are populated from authentication configuration.
    /// </summary>
    [Fact]
    public void AddAuthenticationAuthorization_ConfiguresJwtBearerOptionsFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiAuthenticationOptions.SectionName}:Authority"] = "http://localhost:8080/realms/celeste",
                [$"{ApiAuthenticationOptions.SectionName}:Audience"] = "celeste.inventory",
                [$"{ApiAuthenticationOptions.SectionName}:RequireHttpsMetadata"] = "false",
                [$"{ApiAuthenticationOptions.SectionName}:NameClaimType"] = "preferred_username",
                [$"{ApiAuthenticationOptions.SectionName}:UserIdClaimType"] = "sub"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddAuthenticationAuthorization(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var jwtBearerOptions = serviceProvider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal("http://localhost:8080/realms/celeste", jwtBearerOptions.Authority);
        Assert.Equal("celeste.inventory", jwtBearerOptions.Audience);
        Assert.False(jwtBearerOptions.RequireHttpsMetadata);
        Assert.Equal("preferred_username", jwtBearerOptions.TokenValidationParameters.NameClaimType);
    }

    /// <summary>
    ///     Ensures required authentication configuration is validated.
    /// </summary>
    [Fact]
    public void AddAuthenticationAuthorization_WhenAuthorityIsMissing_ThrowsOptionsValidationException()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiAuthenticationOptions.SectionName}:Authority"] = string.Empty,
                [$"{ApiAuthenticationOptions.SectionName}:Audience"] = "celeste.inventory",
                [$"{ApiAuthenticationOptions.SectionName}:RequireHttpsMetadata"] = "false",
                [$"{ApiAuthenticationOptions.SectionName}:NameClaimType"] = "preferred_username",
                [$"{ApiAuthenticationOptions.SectionName}:UserIdClaimType"] = "sub"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddAuthenticationAuthorization(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() =>
            _ = serviceProvider.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value);
    }

    /// <summary>
    ///     Ensures read scope can be resolved from a space-separated scope claim.
    /// </summary>
    [Fact]
    public async Task AddAuthenticationAuthorization_AuthorizesManufacturerRead_FromSpaceSeparatedScopeClaim()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiAuthenticationOptions.SectionName}:Authority"] = "http://localhost:8080/realms/celeste",
                [$"{ApiAuthenticationOptions.SectionName}:Audience"] = "celeste.inventory",
                [$"{ApiAuthenticationOptions.SectionName}:RequireHttpsMetadata"] = "false",
                [$"{ApiAuthenticationOptions.SectionName}:NameClaimType"] = "preferred_username",
                [$"{ApiAuthenticationOptions.SectionName}:UserIdClaimType"] = "sub"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthenticationAuthorization(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim("scope", "email profile manufacturer.read"),
                new Claim("sub", "user-123")
            ],
            JwtBearerDefaults.AuthenticationScheme));

        var result = await authorizationService.AuthorizeAsync(principal, null, AuthorizationPolicies.ManufacturerRead);

        Assert.True(result.Succeeded);
    }

    /// <summary>
    ///     Ensures write scope can be resolved from a space-separated scope claim.
    /// </summary>
    [Fact]
    public async Task AddAuthenticationAuthorization_AuthorizesManufacturerWrite_FromSpaceSeparatedScopeClaim()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiAuthenticationOptions.SectionName}:Authority"] = "http://localhost:8080/realms/celeste",
                [$"{ApiAuthenticationOptions.SectionName}:Audience"] = "celeste.inventory",
                [$"{ApiAuthenticationOptions.SectionName}:RequireHttpsMetadata"] = "false",
                [$"{ApiAuthenticationOptions.SectionName}:NameClaimType"] = "preferred_username",
                [$"{ApiAuthenticationOptions.SectionName}:UserIdClaimType"] = "sub"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthenticationAuthorization(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim("scope", "email profile manufacturer.read manufacturer.write"),
                new Claim("sub", "user-123")
            ],
            JwtBearerDefaults.AuthenticationScheme));

        var result = await authorizationService.AuthorizeAsync(principal, null, AuthorizationPolicies.ManufacturerWrite);

        Assert.True(result.Succeeded);
    }

    /// <summary>
    ///     Ensures write policy fails when the required scope is missing.
    /// </summary>
    [Fact]
    public async Task AddAuthenticationAuthorization_DoesNotAuthorizeManufacturerWrite_WithoutWriteScope()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ApiAuthenticationOptions.SectionName}:Authority"] = "http://localhost:8080/realms/celeste",
                [$"{ApiAuthenticationOptions.SectionName}:Audience"] = "celeste.inventory",
                [$"{ApiAuthenticationOptions.SectionName}:RequireHttpsMetadata"] = "false",
                [$"{ApiAuthenticationOptions.SectionName}:NameClaimType"] = "preferred_username",
                [$"{ApiAuthenticationOptions.SectionName}:UserIdClaimType"] = "sub"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthenticationAuthorization(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim("scope", "email profile manufacturer.read"),
                new Claim("sub", "user-123")
            ],
            JwtBearerDefaults.AuthenticationScheme));

        var result = await authorizationService.AuthorizeAsync(principal, null, AuthorizationPolicies.ManufacturerWrite);

        Assert.False(result.Succeeded);
    }
}
