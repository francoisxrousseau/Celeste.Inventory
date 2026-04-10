namespace Celeste.Inventory.Infrastructure.Installers;

using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Core.Repositories;
using Celeste.Inventory.Infrastructure.Options;
using Celeste.Inventory.Infrastructure.Messaging;
using Celeste.Inventory.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

/// <summary>
///     Registers infrastructure persistence services and database dependencies.
/// </summary>
public static class DatabaseInstaller
{
    /// <summary>
    ///     Adds infrastructure persistence services to the service collection.
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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Database connection string is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Name), "Database name is required.")
            .ValidateOnStart();

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var settings = MongoClientSettings.FromConnectionString(options.ConnectionString);
            settings.ApplicationName = AppDomain.CurrentDomain.FriendlyName;
            return new MongoClient(settings);
        });

        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.Name);
        });

        services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
        services.AddScoped<IManufacturerEventPublisher, ManufacturerEventPublisher>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductEventPublisher, ProductEventPublisher>();

        return services;
    }
}
