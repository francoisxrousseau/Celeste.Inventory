using Celeste.Inventory.Api.Options;
using Celeste.Inventory.Application.Features.Handlers;
using Celeste.Inventory.Events;
using Celeste.Inventory.Infrastructure.Installers;
using Confluent.Kafka;
using Emit.DependencyInjection;
using Emit.Kafka.DependencyInjection;
using Emit.Mediator.DependencyInjection;
using Emit.MongoDB.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Celeste.Inventory.Api.Installers;

/// <summary>
///     Registers manufacturer-specific infrastructure and Emit bootstrap services.
/// </summary>
public static class ManufacturerBootstrapInstaller
{
    /// <summary>
    ///     Adds manufacturer bootstrap registrations to the service collection.
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
    public static IServiceCollection AddManufacturerBootstrap(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaOptions = configuration.GetSection(KafkaOptions.SectionName).Get<KafkaOptions>() ?? new KafkaOptions();

        services
            .AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection(KafkaOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.BootstrapServers), "Kafka bootstrap servers are required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SchemaRegistryUrl), "Kafka schema registry URL is required.")
            .ValidateOnStart();

        services.AddInfrastructure(configuration);

        services.AddEmit(emit =>
        {
            emit.AddMediator(mediator =>
            {
                mediator.AddHandler<CreateManufacturerHandler>();
                mediator.AddHandler<UpdateManufacturerHandler>();
                mediator.AddHandler<DeleteManufacturerHandler>();
                mediator.AddHandler<GetManufacturerByIdHandler>();
                mediator.AddHandler<ListManufacturersHandler>();
                mediator.AddHandler<CountManufacturersHandler>();
            });

            emit.AddMongoDb(mongo =>
            {
                mongo.Configure((serviceProvider, context) =>
                {
                    context.Client = serviceProvider.GetRequiredService<MongoDB.Driver.IMongoClient>();
                    context.Database = serviceProvider.GetRequiredService<MongoDB.Driver.IMongoDatabase>();
                })
                .UseOutbox()
                .UseDistributedLock();
            });

            emit.AddKafka(kafka =>
            {
                kafka.ConfigureClient(config =>
                {
                    config.BootstrapServers = kafkaOptions.BootstrapServers;
                });

                kafka.ConfigureSchemaRegistry(config =>
                {
                    config.Url = kafkaOptions.SchemaRegistryUrl;
                });

                kafka.Topic<string, ManufacturerEvent>(ManufacturerEventTopics.Manufacturer, topic =>
                {
                    topic.SetKeySerializer(Serializers.Utf8);
                    topic.SetKeyDeserializer(Deserializers.Utf8);
                    topic.SetAvroValueSerializer<string, ManufacturerEvent>();
                    topic.SetAvroValueDeserializer<string, ManufacturerEvent>();
                    topic.Producer();
                });
            });
        });

        return services;
    }
}
