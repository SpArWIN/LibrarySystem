using Common.Cached.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Cached.Extensions;

/// <summary>
/// Расширения для кеша.
/// </summary>
public static class CacheExtensions
{
    /// <summary>
    /// Добавить сервисы кеширования.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns>.</returns>
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}