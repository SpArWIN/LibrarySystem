using Common.Contracts.Storage.Settings;
using Common.Messaging.Nats.Extensions;
using Common.Policies.Di;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Storage.Application.Abstractions;
using Storage.Application.Constaints.Section;
using Storage.Application.Services;
using Storage.Domain.Repository;
using Storage.Infrastructure.Repository;

namespace Storage.Application.Extensions;

/// <summary>
/// Расширение на сервисы.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Добавить сервисы хранилища.
    /// </summary>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddStorage(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IImageStorageService, ImageStorageService>();
        serviceCollection.AddTransient<IBucketHelperService, BucketHelperService>();
        serviceCollection.AddTransient<IMinioBucketService, MinioBucketService>();
        serviceCollection.AddNatsStorage(configuration);
        serviceCollection.AddMinio(configuration);
        serviceCollection.AddDefaultPolicies();
        serviceCollection.AddPoliciesService();
        serviceCollection.AddNatsPolicies(configuration);
        return serviceCollection;
    }

    private static IServiceCollection AddMinio(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<MinioOptions>(configuration.GetSection(Section.Minio));
        var minioOptions = configuration.GetSection(Section.Minio).Get<MinioOptions>();
        if (minioOptions is null)
        {
            throw new ApplicationException("Minio configuration section is missing.");
        }

        serviceCollection.AddSingleton<IMinioClient>(sp =>
        {
            var builder = new MinioClient()
                .WithEndpoint(minioOptions.Endpoint, minioOptions.Port)
                .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);

            if (minioOptions.WithSsl)
            {
                builder.WithSSL();
            }

            return builder.Build();
        });
        serviceCollection.AddTransient<IMinioRepository, MinioRepository>();
        serviceCollection.AddTransient<IPublicUrlRewriter, PublicUrlRewriter>();
        return serviceCollection;
    }

    /// <summary>
    /// Добавить Nats для Storage.
    /// </summary>
    /// <param name="serviceCollection"><see cref="ServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddNatsStorage(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddNats(configuration);
        serviceCollection.AddSingleton<INatsStorage, NatsStorage>();
        return serviceCollection;
    }
}