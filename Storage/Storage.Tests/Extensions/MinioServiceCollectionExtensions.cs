using Common.Contracts.Storage.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Storage.Application.Constaints.Section;
using Storage.Domain.Repository;
using Storage.Infrastructure.Repository;

namespace Storage.Tests.Extensions;

public static class MinioServiceCollectionExtensions
{
    /// <summary>
    /// Добавить в DI тестовый MINIO
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="hostname">Хост.</param>
    /// <param name="port">Порт.</param>
    /// <param name="accessKey">Ключ.</param>
    /// <param name="secretKey">Секретный ключ.</param>
    /// <param name="defaultBucket">Бакет.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddTestsMinio(
        this IServiceCollection services,
        string hostname,
        int port,
        string accessKey = "minioadmin",
        string secretKey = "minioadmin",
        string defaultBucket = "test-bucket")
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Minio:Endpoint"] = hostname,
                ["Minio:Port"] = port.ToString(),
                ["Minio:WithSsl"] = "false",
                ["Minio:AccessKey"] = accessKey,
                ["Minio:SecretKey"] = secretKey,
                ["Minio:DefaultBucket"] = defaultBucket
            }!)
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<MinioOptions>(configuration.GetSection(Section.Minio));
        services.AddScoped<IPublicUrlRewriter, PublicUrlRewriter>();
        services.AddScoped<IMinioRepository, MinioRepository>();
        services.AddSingleton<IMinioClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
            return CreateMinioClient(options);
        });
        
        return services;
    }

    private static IMinioClient CreateMinioClient(MinioOptions options)
    {
        var builder = new MinioClient()
            .WithEndpoint(options.Endpoint, options.Port)
            .WithCredentials(options.AccessKey, options.SecretKey);
        
        if (options.WithSsl)
        {
            builder.WithSSL();
        }
        
        return builder.Build();
    }
}