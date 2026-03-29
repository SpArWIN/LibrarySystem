using Common.Contracts.Constaints.Sections;
using Common.Messaging.Nats.Clients;
using Common.Messaging.Nats.Factories.Consumer;
using Common.Messaging.Nats.Factories.Producer;
using Common.Messaging.Nats.Serialize;
using Common.Messaging.Nats.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using INatsClient = Common.Messaging.Nats.Clients.INatsClient;
namespace Common.Messaging.Nats.Extensions;

/// <summary>
/// Расширение на Nats.
/// </summary>
public static class NatsExtensions
{
    /// <summary>
    /// Добавить поддержку Nats.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddNats(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<NatsConnectionOptions>()
            .Bind(configuration.GetSection(Section.Nats))
            .ValidateDataAnnotations();

        services.AddOptions<NatsJetStreamOptions>()
            .Bind(configuration.GetSection(Section.JetStream))
            .ValidateDataAnnotations();
        
        services.AddNatsSerializer();
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<NatsConnectionOptions>>().Value);
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<NatsJetStreamOptions>>().Value);
        
        services.AddSingleton<INatsClient, NatsClient>();
        services.AddSingleton<INatsProducerFactory, NatsProducerFactory>();
        services.AddSingleton<INatsConsumerFactory, NatsConsumerFactory>();
        return services;
    }

    private static IServiceCollection AddNatsSerializer(this IServiceCollection services)
    {
       services.AddSingleton(typeof(INatsSerializer<>), typeof(NatsSerialize<>));
       services.AddSingleton(typeof(INatsDeserialize<>), typeof(NatsDeserialize<>));
       return services;
    }
}