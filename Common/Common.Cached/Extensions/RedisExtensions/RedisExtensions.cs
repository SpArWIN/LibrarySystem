using System.Reflection;
using BloomFilter;
using BloomFilter.Redis;
using Common.Cached.Configurations;
using Common.Cached.PrefixStrategy;
using Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Common.Cached.Extensions.RedisExtensions;

/// <summary>
/// Расширение на Redis.
/// </summary>
public static class RedisExtensions
{
    /// <summary>
    /// Добавить кеширование Redis.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.ConfigureAndValidate<RedisCacheOptions>(
            configuration.GetSection(RedisCacheOptions.SectionName));
        services.ConfigureAndAdd<CachePrefixesOptions>(
            configuration.GetSection(CachePrefixesOptions.SectionName));
        
        services.AddCachePrefixStrategy();

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var options = provider.GetRequiredService<RedisCacheOptions>();
            return ConnectionMultiplexer.Connect(options.ConnectionString);
        });
        return services;
    }

    /// <summary>
    /// Регистрация стретигии получения префикса.
    /// </summary>
    /// <returns></returns>
    private static IServiceCollection AddCachePrefixStrategy(this IServiceCollection services)
    {
        var strategyTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        typeof(ICachePrefixStrategy).IsAssignableFrom(t));
        strategyTypes.ForEach(strategyType =>
        {
            services.AddSingleton(typeof(ICachePrefixStrategy), strategyType);
        });
        services.AddSingleton<ICachedResolver, CachedPrefixResolver>();
        return services;
    }

    /// <summary>
    /// Redis-backed Bloom filter для проверки «возможного» наличия пользователя.
    /// </summary>
    public static IServiceCollection AddBloomUserFilter(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<BloomUserFilterOptions>(
            configuration.GetSection(BloomUserFilterOptions.SectionName));
        
        services.AddSingleton<IBloomFilter>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BloomUserFilterOptions>>().Value;
            var redisOptions = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

            if (!options.Enabled)
            {
                return FilterBuilder
                    .Create()
                    .WithName("UserBloomFilter-disabled")
                    .ExpectingElements(1)
                    .WithErrorRate(0.01)
                    .BuildInMemory();
            }

            var mux = provider.GetRequiredService<IConnectionMultiplexer>();
            var prefix = options.InstanceName ?? redisOptions.InstanceName;
            var redisKey = $"{prefix}{options.RedisKey}";

            return FilterRedisBuilder.Build(
                mux,
                redisKey,
                options.ExpectedElements,
                options.ErrorRate,
                "UserBloomFilter");
        });

        return services;
    }
}
