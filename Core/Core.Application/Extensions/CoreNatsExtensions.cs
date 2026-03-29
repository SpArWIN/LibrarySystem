using Common.Cached.Extensions;
using Common.Contracts.Constaints.Sections;
using Common.Messaging.Nats.Extensions;
using Core.Application.Handlers.Storage;
using Core.Application.Hosted;
using Core.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

/// <summary>
/// NATS / JetStream для Core (консьюмеры, кеш предзагрузки).
/// </summary>
public static class CoreNatsExtensions
{
    /// <summary>
    /// Регистрирует обработчики событий NATS и фоновую подписку JetStream.
    /// </summary>
    public static IServiceCollection AddCoreNatsConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PreloadedImageCacheOptions>(configuration.GetSection(Section.PreloadedImageCache));
        services.AddCacheServices();
        services.AddNatsMessageHandlers(typeof(PreloadedImageAddedHandler).Assembly);
        services.AddHostedService<PreloadedImagedService>();
        return services;
    }
}
