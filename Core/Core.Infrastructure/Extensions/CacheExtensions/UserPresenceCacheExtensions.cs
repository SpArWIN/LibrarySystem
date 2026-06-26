using Common.Cached.Extensions.RedisExtensions;
using Core.Domain.Repository;
using Core.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions.CacheExtensions;

/// <summary>
/// Расширение на <see cref="IUserPresenceCache"/>
/// </summary>
public static class UserPresenceCacheExtensions
{
    /// <summary>
    /// Redis, Bloom filter и <see cref="IUserPresenceCache"/>.
    /// </summary>
    public static IServiceCollection AddUserPresenceCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedis(configuration);
        services.AddBloomUserFilter(configuration);
        services.AddTransient<IUserPresenceCache, UserPresenceCache>();
        return services;
    }
}