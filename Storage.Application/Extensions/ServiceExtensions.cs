using Common.Contracts.Storage.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        serviceCollection.AddMinio(configuration);
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
        serviceCollection.AddTransient<IMinioRepository, MinioRepository>();
        return serviceCollection;
    }
}